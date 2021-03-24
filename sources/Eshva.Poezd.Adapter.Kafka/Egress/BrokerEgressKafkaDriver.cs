#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Broker egress Kafka driver.
  /// </summary>
  internal class BrokerEgressKafkaDriver : IBrokerEgressDriver
  {
    /// <summary>
    /// Constructs a new instance of broker egress Kafka driver.
    /// </summary>
    /// <param name="driverConfiguration">
    /// The driver configuration.
    /// </param>
    /// <param name="producerRegistry">
    /// The producer registry.
    /// </param>
    public BrokerEgressKafkaDriver(
      [NotNull] BrokerEgressKafkaDriverConfiguration driverConfiguration,
      [NotNull] IProducerRegistry producerRegistry)
    {
      _driverConfiguration = driverConfiguration ?? throw new ArgumentNullException(nameof(driverConfiguration));
      _producerRegistry = producerRegistry ?? throw new ArgumentNullException(nameof(producerRegistry));
    }

    /// <inheritdoc />
    public void Initialize(
      string brokerId,
      IEnumerable<IEgressApi> apis,
      IDiContainerAdapter serviceProvider)
    {
      if (_isInitialized) throw new PoezdOperationException($"Kafka driver for broker with ID {_brokerId} is already initialized.");
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));

      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _serializerFactory = _serviceProvider.GetService<ISerializerFactory>(_driverConfiguration.SerializerFactoryType);
      _producerFactory = _serviceProvider.GetService<IProducerFactory>(_driverConfiguration.ProducerFactoryType);
      _producerConfigurator = _serviceProvider.GetService<IProducerConfigurator>(_driverConfiguration.ProducerConfiguratorType);
      _headerValueCodec = _serviceProvider.GetService<IHeaderValueCodec>(_driverConfiguration.HeaderValueCodecType);
      _loggerFactory = _serviceProvider.GetService<ILoggerFactory>();
      _apis = apis ?? throw new ArgumentNullException(nameof(apis));
      _brokerId = brokerId;

      CreateAndRegisterProducerPerApi();

      _isInitialized = true;
    }

    /// <inheritdoc />
    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken)
    {
      if (!_isInitialized) throw new PoezdOperationException("Kafka driver should be initialized before it can publish messages.");
      if (context == null) throw new ArgumentNullException(nameof(context));

      return _producerRegistry.Get(context.Api).Publish(context, cancellationToken);
    }

    /// <inheritdoc />
    public void Dispose() => _producerRegistry.Dispose();

    private void CreateAndRegisterProducerPerApi()
    {
      foreach (var api in _apis)
      {
        var concreteMethod = CreateAndRegisterProducerMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object[] {api});
      }
    }

    private void CreateAndRegisterProducer<TKey, TValue>(IEgressApi api)
    {
      var producer = new DefaultApiProducer<TKey, TValue>(
        _producerFactory.Create<TKey, TValue>(
          _driverConfiguration.ProducerConfig,
          _producerConfigurator,
          _serializerFactory),
        _headerValueCodec,
        _loggerFactory.CreateLogger<DefaultApiProducer<TKey, TValue>>());
      _producerRegistry.Add(api, producer);
    }

    private readonly BrokerEgressKafkaDriverConfiguration _driverConfiguration;
    private readonly IProducerRegistry _producerRegistry;
    private IEnumerable<IEgressApi> _apis;
    private string _brokerId;
    private IHeaderValueCodec _headerValueCodec;
    private bool _isInitialized;
    private ILoggerFactory _loggerFactory;
    private IProducerConfigurator _producerConfigurator;
    private IProducerFactory _producerFactory;
    private ISerializerFactory _serializerFactory;
    private IDiContainerAdapter _serviceProvider;

    private static readonly MethodInfo CreateAndRegisterProducerMethod =
      typeof(BrokerEgressKafkaDriver).GetMethod(nameof(CreateAndRegisterProducer), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
