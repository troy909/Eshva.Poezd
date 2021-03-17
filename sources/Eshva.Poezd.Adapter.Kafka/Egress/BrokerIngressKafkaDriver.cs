#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  internal class BrokerIngressKafkaDriver : IBrokerIngressDriver
  {
    public BrokerIngressKafkaDriver(
      [NotNull] BrokerIngressKafkaDriverConfiguration configuration,
      [NotNull] IConsumerRegistry consumerRegistry)
    {
      _driverConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _consumerRegistry = consumerRegistry ?? throw new ArgumentNullException(nameof(consumerRegistry));

      const string message = "Ingress Kafka driver configuration missing ";
      if (_driverConfiguration.ConsumerConfiguratorType == null)
        throw new ArgumentException(message + "consumer configurator type.", nameof(configuration));
      if (_driverConfiguration.ConsumerFactoryType == null)
        throw new ArgumentException(message + "consumer factory type.", nameof(configuration));
      if (_driverConfiguration.DeserializerFactoryType == null)
        throw new ArgumentException(message + "deserializer factory type.", nameof(configuration));
      if (_driverConfiguration.HeaderValueCodecType == null)
        throw new ArgumentException(message + "header value codec type.", nameof(configuration));
    }

    /// <inheritdoc />
    public void Initialize(
      string brokerId,
      IMessageRouter messageRouter,
      IEnumerable<IIngressApi> apis,
      IServiceProvider serviceProvider)
    {
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));
      _brokerId = brokerId;
      _messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _apis = apis ?? throw new ArgumentNullException(nameof(apis));

      if (_isInitialized)
        throw new PoezdOperationException($"Kafka driver for broker with ID '{_brokerId}' is already initialized.");

      GetRequiredServices();
      CreateAndRegisterConsumerPerApi();

      _isInitialized = true;
    }

    /// <inheritdoc />
    public Task StartConsumeMessages(
      IEnumerable<string> queueNamePatterns,
      CancellationToken cancellationToken = default)
    {
      if (!_isInitialized)
      {
        throw new PoezdOperationException(
          "Broker ingress Kafka driver should be initialized before it can consume messages.");
      }

      if (queueNamePatterns == null) throw new ArgumentNullException(nameof(queueNamePatterns));

      StartConsumeMessagesFromApis(cancellationToken);
      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose() => _consumerRegistry.Dispose();

    private void GetRequiredServices()
    {
      _consumerConfigurator = GetService<IConsumerConfigurator>(_driverConfiguration.ConsumerConfiguratorType);
      _consumerFactory = GetService<IConsumerFactory>(_driverConfiguration.ConsumerFactoryType);
      _deserializerFactory = GetService<IDeserializerFactory>(_driverConfiguration.DeserializerFactoryType);
      _headerValueCodecType = GetService<IHeaderValueCodec>(_driverConfiguration.HeaderValueCodecType);
      _loggerFactory = GetService<ILoggerFactory>(typeof(ILoggerFactory));
    }

    private TResult GetService<TResult>(Type serviceType)
    {
      // ReSharper disable once NotResolvedInText
      var service = _serviceProvider.GetService(serviceType) ?? throw new ArgumentException(
        $"Service of type {serviceType.FullName} is missing in your DI-container.",
        "serviceProvider");
      return (TResult) service;
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
      var apiConsumer = new ApiConsumer<TKey, TValue>(
        api,
        _consumerFactory.Create<TKey, TValue>(
          _driverConfiguration.ConsumerConfig,
          _consumerConfigurator,
          _deserializerFactory),
        _loggerFactory.CreateLogger<ApiConsumer<TKey, TValue>>());
      _consumerRegistry.Add(api, apiConsumer);
    }

    private void StartConsumeMessagesFromApis(CancellationToken cancellationToken)
    {
      foreach (var api in _apis)
      {
        var concreteMethod = StartConsumeFromApiTopicsMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object?[] {api, cancellationToken});
      }
    }

    private void StartConsumeFromApiTopics<TKey, TValue>(IIngressApi api, CancellationToken cancellationToken) =>
      _consumerRegistry.Get<TKey, TValue>(api).Start(OnMessageReceived, cancellationToken);

    private async Task OnMessageReceived<TKey, TValue>(ConsumeResult<TKey, TValue> consumeResult)
    {
      // TODO: handle in parallel? No we need a strategy.
      var headers = consumeResult.Message.Headers.ToDictionary(
        header => header.Key,
        header => _headerValueCodecType.Decode(header.GetValueBytes()));
      await _messageRouter.RouteIngressMessage(
        _brokerId,
        consumeResult.Topic,
        consumeResult.Message.Timestamp.UtcDateTime,
        consumeResult.Message.Key,
        consumeResult.Message.Value,
        headers);
    }

    private readonly IConsumerRegistry _consumerRegistry;
    private readonly BrokerIngressKafkaDriverConfiguration _driverConfiguration;
    private IEnumerable<IIngressApi> _apis;
    private string _brokerId;
    private IConsumerConfigurator _consumerConfigurator;
    private IConsumerFactory _consumerFactory;
    private IDeserializerFactory _deserializerFactory;
    private IHeaderValueCodec _headerValueCodecType;
    private bool _isInitialized;
    private ILoggerFactory _loggerFactory;
    private IMessageRouter _messageRouter;
    private IServiceProvider _serviceProvider;

    private static readonly MethodInfo CreateAndRegisterConsumerMethod =
      typeof(BrokerIngressKafkaDriver).GetMethod(nameof(CreateAndRegisterConsumer), BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo StartConsumeFromApiTopicsMethod =
      typeof(BrokerIngressKafkaDriver).GetMethod(nameof(StartConsumeFromApiTopics), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
