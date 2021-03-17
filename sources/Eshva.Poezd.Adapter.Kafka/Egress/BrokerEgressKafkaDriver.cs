#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  public class BrokerEgressKafkaDriver : IBrokerEgressDriver
  {
    internal BrokerEgressKafkaDriver(
      [NotNull] BrokerEgressKafkaDriverConfiguration driverConfiguration,
      [NotNull] IProducerRegistry producerRegistry)
    {
      _driverConfiguration = driverConfiguration ?? throw new ArgumentNullException(nameof(driverConfiguration));
      _producerRegistry = producerRegistry ?? throw new ArgumentNullException(nameof(producerRegistry));
    }

    /// <inheritdoc />
    public void Initialize(
      string brokerId,
      ILogger<IBrokerEgressDriver> logger,
      IClock clock,
      IEnumerable<IEgressApi> apis,
      IServiceProvider serviceProvider)
    {
      if (_isInitialized) throw new PoezdOperationException($"Kafka driver for broker with ID {_brokerId} is already initialized.");
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));

      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _serializerFactory = (ISerializerFactory) _serviceProvider.GetService(_driverConfiguration.SerializerFactoryType);
      _producerFactory = (IProducerFactory) _serviceProvider.GetService(_driverConfiguration.ProducerFactoryType);
      _producerConfigurator = (IProducerConfigurator) _serviceProvider.GetService(_driverConfiguration.ProducerConfiguratorType);
      _apis = apis;
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _clock = clock ?? throw new ArgumentNullException(nameof(clock));
      _brokerId = brokerId;

      CreateAndRegisterProducerPerApi();

      _isInitialized = true;
    }

    /// <inheritdoc />
    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken)
    {
      if (!_isInitialized) throw new PoezdOperationException("Kafka driver should be initialized before it can publish messages.");
      if (context == null) throw new ArgumentNullException(nameof(context));
      var errors = ValidateContext(context);
      if (!string.IsNullOrWhiteSpace(errors)) throw new PoezdOperationException($"Message publishing context has missing data: {errors}.");

      var publish = PublishMethod!.MakeGenericMethod(context.Key.GetType(), context.Payload.GetType());
      return ((Task) publish.Invoke(this, new object[] {context, cancellationToken}))!;
    }

    /// <inheritdoc />
    public void Dispose() => _producerRegistry.Dispose();

    private void CreateAndRegisterProducerPerApi()
    {
      foreach (var api in _apis)
      {
        var concreteMethod = CreateAndRegisterProducerMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object?[] {api});
      }
    }

    private void CreateAndRegisterProducer<TKey, TValue>(IEgressApi api)
    {
      var producer = _producerFactory.Create<TKey, TValue>(
        _driverConfiguration.ProducerConfig,
        _producerConfigurator,
        _serializerFactory);
      _producerRegistry.Add(api, producer);
    }

    private static string ValidateContext(MessagePublishingContext context)
    {
      var errors = new List<string>();
      if (context.Metadata == null) errors.Add(nameof(context.Metadata));
      if (context.Payload == null) errors.Add(nameof(context.Payload));
      if (context.Key == null) errors.Add(nameof(context.Key));
      if (context.Api == null) errors.Add(nameof(context.Api));
      if (context.QueueNames == null) errors.Add(nameof(context.QueueNames));
      if (context.Broker == null) errors.Add(nameof(context.Broker));

      return string.Join(", ", errors);
    }

    private async Task Publish<TKey, TValue>(MessagePublishingContext context, CancellationToken cancellationToken)
    {
      var producer = _producerRegistry.Get<TKey, TValue>(context.Api);
      var headers = MakeHeaders(context.Metadata);
      var timestamp = new Timestamp(_clock.GetNowUtc());
      var brokerId = context.Broker.Id;
      foreach (var queueName in context.QueueNames)
      {
        var key = (TKey) context.Key;
        var payload = (TValue) context.Payload;
        var message = new Message<TKey, TValue> {Key = key, Value = payload, Headers = headers, Timestamp = timestamp};
        try
        {
          _logger.LogDebug(
            "Publishing a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
            key,
            queueName,
            brokerId);
          await producer.ProduceAsync(
            queueName,
            message,
            cancellationToken);
          _logger.LogDebug(
            "Successfully published a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
            key,
            queueName,
            brokerId);
        }
        catch (Exception exception)
        {
          _logger.LogDebug(
            exception,
            "Failed to publish a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
            key,
            queueName,
            brokerId);
          throw;
        }
      }
    }

    private static Headers MakeHeaders(IReadOnlyDictionary<string, string> metadata)
    {
      var headers = new Headers();
      foreach (var (key, value) in metadata)
      {
        headers.Add(key, Encoding.UTF8.GetBytes(value));
      }

      return headers;
    }

    private readonly BrokerEgressKafkaDriverConfiguration _driverConfiguration;
    private readonly IProducerRegistry _producerRegistry;
    private IEnumerable<IEgressApi> _apis;
    private string _brokerId;
    private IClock _clock;
    private bool _isInitialized;
    private ILogger<IBrokerEgressDriver> _logger;
    private IProducerConfigurator _producerConfigurator;
    private IProducerFactory _producerFactory;
    private ISerializerFactory _serializerFactory;
    private IServiceProvider _serviceProvider;

    private static readonly MethodInfo PublishMethod =
      typeof(BrokerEgressKafkaDriver).GetMethod(nameof(Publish), BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo CreateAndRegisterProducerMethod =
      typeof(BrokerEgressKafkaDriver).GetMethod(nameof(CreateAndRegisterProducer), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
