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
  /// <summary>
  /// Broker ingress Kafka driver.
  /// </summary>
  internal class BrokerIngressKafkaDriver : IBrokerIngressDriver
  {
    /// <summary>
    /// Constructs a new instance of broker ingress Kafka driver.
    /// </summary>
    /// <param name="configuration">
    /// The driver configuration.
    /// </param>
    /// <param name="consumerRegistry">
    /// The consumer registry used to manage Kafka consumers.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of parameters is not specified.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// One of required driver configuration fields is not specified.
    /// </exception>
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
      IBrokerIngress brokerIngress,
      IEnumerable<IIngressApi> apis,
      IDiContainerAdapter serviceProvider)
    {
      _brokerIngress = brokerIngress ?? throw new ArgumentNullException(nameof(brokerIngress));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _apis = apis ?? throw new ArgumentNullException(nameof(apis));

      if (_isInitialized)
        throw new PoezdOperationException("Kafka ingress driver is already initialized.");

      GetRequiredServices();
      CreateAndRegisterConsumerPerApi();

      _isInitialized = true;
    }

    /// <inheritdoc />
    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
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
      _consumerConfigurator = _serviceProvider.GetService<IConsumerConfigurator>(_driverConfiguration.ConsumerConfiguratorType);
      _consumerFactory = _serviceProvider.GetService<IConsumerFactory>(_driverConfiguration.ConsumerFactoryType);
      _deserializerFactory = _serviceProvider.GetService<IDeserializerFactory>(_driverConfiguration.DeserializerFactoryType);
      _headerValueCodecType = _serviceProvider.GetService<IHeaderValueCodec>(_driverConfiguration.HeaderValueCodecType);
      _loggerFactory = _serviceProvider.GetService<ILoggerFactory>(typeof(ILoggerFactory));
    }

    private void CreateAndRegisterConsumerPerApi()
    {
      foreach (var api in _apis)
      {
        var concreteMethod = CreateAndRegisterConsumerMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object[] {api});
      }
    }

    private void CreateAndRegisterConsumer<TKey, TValue>(IIngressApi api)
    {
      var apiConsumer = new DefaultApiConsumer<TKey, TValue>(
        api,
        _consumerFactory.Create<TKey, TValue>(
          _driverConfiguration.ConsumerConfig,
          _consumerConfigurator,
          _deserializerFactory),
        _loggerFactory.CreateLogger<DefaultApiConsumer<TKey, TValue>>());
      _consumerRegistry.Add(api, apiConsumer);
    }

    private void StartConsumeMessagesFromApis(CancellationToken cancellationToken)
    {
      foreach (var api in _apis)
      {
        var concreteMethod = StartConsumeFromApiTopicsMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object[] {api, cancellationToken});
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
      await _brokerIngress.RouteIngressMessage(
        consumeResult.Topic,
        consumeResult.Message.Timestamp.UtcDateTime,
        consumeResult.Message.Key,
        consumeResult.Message.Value,
        headers);
    }

    private readonly IConsumerRegistry _consumerRegistry;
    private readonly BrokerIngressKafkaDriverConfiguration _driverConfiguration;
    private IEnumerable<IIngressApi> _apis;
    private IBrokerIngress _brokerIngress;
    private IConsumerConfigurator _consumerConfigurator;
    private IConsumerFactory _consumerFactory;
    private IDeserializerFactory _deserializerFactory;
    private IHeaderValueCodec _headerValueCodecType;
    private bool _isInitialized;
    private ILoggerFactory _loggerFactory;
    private IDiContainerAdapter _serviceProvider;

    private static readonly MethodInfo CreateAndRegisterConsumerMethod =
      typeof(BrokerIngressKafkaDriver).GetMethod(nameof(CreateAndRegisterConsumer), BindingFlags.Instance | BindingFlags.NonPublic);

    private static readonly MethodInfo StartConsumeFromApiTopicsMethod =
      typeof(BrokerIngressKafkaDriver).GetMethod(nameof(StartConsumeFromApiTopics), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
