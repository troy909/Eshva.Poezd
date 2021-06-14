#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

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
    public void Initialize(IEnumerable<IEgressApi> apis, IDiContainerAdapter serviceProvider)
    {
      if (_isInitialized)
      {
        throw new PoezdOperationException(
          $"Kafka driver for broker with bootstrap servers{_driverConfiguration.ProducerConfig.BootstrapServers} is already initialized.");
      }

      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _apis = apis ?? throw new ArgumentNullException(nameof(apis));

      GetRequiredServices();
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

    private void GetRequiredServices()
    {
      _apiProducerFactory = _serviceProvider.GetService<IApiProducerFactory>(_driverConfiguration.ProducerFactoryType);
    }

    private void CreateAndRegisterProducerPerApi()
    {
      foreach (var api in _apis)
      {
        var concreteMethod = CreateApiProducerMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        var producer = (IApiProducer) concreteMethod.Invoke(this, new object[] { });
        _producerRegistry.Add(api, producer);
      }
    }

    private IApiProducer CreateApiProducer<TKey, TValue>() =>
      _apiProducerFactory.Create<TKey, TValue>(_driverConfiguration.ProducerConfig);

    private readonly BrokerEgressKafkaDriverConfiguration _driverConfiguration;
    private readonly IProducerRegistry _producerRegistry;
    private IApiProducerFactory _apiProducerFactory;
    private IEnumerable<IEgressApi> _apis;
    private bool _isInitialized;
    private IDiContainerAdapter _serviceProvider;

    private static readonly MethodInfo CreateApiProducerMethod =
      typeof(BrokerEgressKafkaDriver).GetMethod(nameof(CreateApiProducer), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
