#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  internal class BrokerIngressKafkaDriver : IBrokerIngressDriver
  {
    public BrokerIngressKafkaDriver(
      [NotNull] BrokerIngressKafkaDriverConfiguration configuration,
      [NotNull] IConsumerRegistry consumerRegistry)
    {
      _driverConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _consumerRegistry = consumerRegistry ?? throw new ArgumentNullException(nameof(consumerRegistry));
    }

    /// <inheritdoc />
    public void Initialize(
      string brokerId,
      IMessageRouter messageRouter,
      IEnumerable<IIngressApi> apis,
      IServiceProvider serviceProvider)
    {
      _messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _deserializerFactory = (IDeserializerFactory) serviceProvider.GetService(_driverConfiguration.DeserializerFactoryType);
      _consumerFactory = (IConsumerFactory) serviceProvider.GetService(_driverConfiguration.ConsumerFactoryType);
      _logger = (ILogger<BrokerIngressKafkaDriver>) serviceProvider.GetService(typeof(ILogger<BrokerIngressKafkaDriver>)) ??
                throw new PoezdConfigurationException(
                  $"Can not get a logger of type {typeof(ILogger<BrokerIngressKafkaDriver>).FullName}.");

      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));
      _brokerId = brokerId;

      if (_isInitialized)
        throw new PoezdOperationException($"Kafka driver for broker with ID '{_brokerId}' is already initialized.");

      _apis = apis;
      CreateAndRegisterConsumerPerApi();

      _isInitialized = true;
    }

    /// <inheritdoc />
    public Task StartConsumeMessages(
      IEnumerable<string> queueNamePatterns,
      CancellationToken cancellationToken = default)
    {
      if (!_isInitialized) throw new PoezdOperationException("Kafka driver should be initialized before it can consume messages.");

      if (queueNamePatterns == null) throw new ArgumentNullException(nameof(queueNamePatterns));

      StartConsumeMessagesFromApis(cancellationToken);
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
      StopMessageConsumption();
      _consumerRegistry.Dispose();
    }

    private void StopMessageConsumption()
    {
      foreach (var api in _apis)
      {
        var concreteMethod = StopConsumerOfApiMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object?[] {api});
      }
    }

    private void StopConsumerOfApi<TKey, TValue>(IIngressApi api)
    {
      var consumer = _consumerRegistry.Get<TKey, TValue>(api);
      try
      {
        consumer.Commit();
      }
      catch (Exception exception)
      {
        // Mostly ignore.
        _logger.LogInformation("During the final offsets commit an error occurred: @{Exception}", exception);
      }

      consumer.Close();
      _logger.LogInformation(
        "Closed consumer with bootstrap servers '@{BootstrapServers}' and @{GroupID}.",
        _driverConfiguration.ConsumerConfig.BootstrapServers,
        _driverConfiguration.ConsumerConfig.GroupId);
    }

    private void StartConsumeMessagesFromApis(CancellationToken cancellationToken)
    {
      foreach (var api in _apis)
      {
        var concreteMethod = StartConsumeFromApiTopicsMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object?[] {api, cancellationToken});
      }
    }

    private void StartConsumeFromApiTopics<TKey, TValue>(IIngressApi api, CancellationToken cancellationToken)
    {
      var consumer = _consumerRegistry.Get<TKey, TValue>(api);
      consumer.Subscribe(api.GetQueueNamePatterns());
      Consume(
        consumer,
        OnMessageReceived,
        cancellationToken);
    }

    private void CreateAndRegisterConsumerPerApi()
    {
      foreach (var api in _apis)
      {
        var concreteMethod = CreateAndRegisterConsumerMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object?[] {api});
      }
    }

    private void CreateAndRegisterConsumer<TKey, TValue>(IIngressApi api)
    {
      var consumerConfigurator = (IConsumerConfigurator) _serviceProvider.GetService(_driverConfiguration.ConsumerConfiguratorType);
      var consumer = _consumerFactory.Create<TKey, TValue>(
        _driverConfiguration.ConsumerConfig,
        consumerConfigurator,
        _deserializerFactory);
      _consumerRegistry.Add(api, consumer);
    }

    private async Task OnMessageReceived<TKey, TValue>(ConsumeResult<TKey, TValue> consumeResult)
    {
      // TODO: handle in parallel? No we need a strategy.
      if (!(_serviceProvider.GetService(_driverConfiguration.HeaderValueParserType) is IHeaderValueParser parser))
      {
        throw new PoezdOperationException(
          "Can not parse Kafka broker message headers because can not get a header value parser. " +
          $"You should register a service of type {_driverConfiguration.HeaderValueParserType.FullName} in your DI-container.");
      }

      var headers = consumeResult.Message.Headers.ToDictionary(
        header => header.Key,
        header => parser.Parser(header.GetValueBytes()));
      await _messageRouter.RouteIngressMessage(
        _brokerId,
        consumeResult.Topic,
        consumeResult.Message.Timestamp.UtcDateTime,
        consumeResult.Message.Key,
        consumeResult.Message.Value,
        headers);
    }

    private void Consume<TKey, TValue>(
      IConsumer<TKey, TValue> consumer,
      Func<ConsumeResult<TKey, TValue>, Task> onMessageReceived,
      CancellationToken cancellationToken = default)
    {
      var consumeTask = Task.Factory.StartNew(
        async () =>
        {
          try
          {
            while (!cancellationToken.IsCancellationRequested)
            {
              var consumeResult = consumer.Consume(cancellationToken);
              if (consumeResult.IsPartitionEOF) continue;

              await onMessageReceived(consumeResult);
              _logger.LogDebug(
                "Received message at @{Offset}: @{Value}",
                consumeResult.TopicPartitionOffset,
                consumeResult.Message.Value);

              // TODO: Choose how to commit received messages.
              // if (consumeResult.Offset.Value % _configuration.CommitPeriod == 0)
              // The Commit method sends a "commit offsets" request to the Kafka
              // cluster and synchronously waits for the response. This is very
              // slow compared to the rate at which the consumer is capable of
              // consuming messages. A high performance application will typically
              // commit offsets relatively infrequently and be designed handle
              // duplicate messages in the event of failure.
              consumer.Commit(consumeResult);
            }
          }
          catch (ConsumeException exception)
          {
            _logger.LogError(
              exception,
              "A Kafka consume error occurred: {Error}",
              exception.Error);
          }
          finally
          {
            _logger.LogInformation("Closing Kafka consumer.");
            consumer.Close();
            _logger.LogInformation("Kafka consumer closed.");
          }
        },
        cancellationToken,
        TaskCreationOptions.LongRunning,
        TaskScheduler.Default
      );
    }

    private readonly IConsumerRegistry _consumerRegistry;
    private readonly BrokerIngressKafkaDriverConfiguration _driverConfiguration;
    private IEnumerable<IIngressApi> _apis;
    private string _brokerId;
    private IConsumerFactory _consumerFactory;
    private IEnumerable<Task<Task>> _consumeTasks;
    private IDeserializerFactory _deserializerFactory;
    private bool _isInitialized;
    private ILogger<BrokerIngressKafkaDriver> _logger;
    private IMessageRouter _messageRouter;
    private IServiceProvider _serviceProvider;

    private static readonly MethodInfo CreateAndRegisterConsumerMethod =
      typeof(BrokerIngressKafkaDriver).GetMethod(nameof(CreateAndRegisterConsumer), BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo StartConsumeFromApiTopicsMethod =
      typeof(BrokerIngressKafkaDriver).GetMethod(nameof(StartConsumeFromApiTopics), BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo StopConsumerOfApiMethod =
      typeof(BrokerIngressKafkaDriver).GetMethod(nameof(StopConsumerOfApi), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
