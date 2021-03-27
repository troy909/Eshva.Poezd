#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// Default API consumer factory.
  /// </summary>
  public class DefaultApiConsumerFactory : IApiConsumerFactory
  {
    /// <summary>
    /// Creates a new instance of default API consumer factory.
    /// </summary>
    /// <param name="consumerFactory">
    /// The native consumer factory.
    /// </param>
    /// <param name="consumerConfigurator">
    /// The native consumer configurator.
    /// </param>
    /// <param name="deserializerFactory">
    /// The deserializer factory.
    /// </param>
    /// <param name="loggerFactory">
    /// The logger factory.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public DefaultApiConsumerFactory(
      [NotNull] IConsumerFactory consumerFactory,
      [NotNull] IConsumerConfigurator consumerConfigurator,
      [NotNull] IDeserializerFactory deserializerFactory,
      [NotNull] ILoggerFactory loggerFactory)
    {
      _consumerFactory = consumerFactory ?? throw new ArgumentNullException(nameof(consumerFactory));
      _consumerConfigurator = consumerConfigurator ?? throw new ArgumentNullException(nameof(consumerConfigurator));
      _deserializerFactory = deserializerFactory ?? throw new ArgumentNullException(nameof(deserializerFactory));
      _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    /// <inheritdoc />
    public IApiConsumer<TKey, TValue> Create<TKey, TValue>(ConsumerConfig config, IIngressApi api)
    {
      var consumer = _consumerFactory.Create<TKey, TValue>(
        config ?? throw new ArgumentNullException(nameof(config)),
        _consumerConfigurator,
        _deserializerFactory);

      return new DefaultApiConsumer<TKey, TValue>(
        api ?? throw new ArgumentNullException(nameof(api)),
        consumer,
        _loggerFactory.CreateLogger<DefaultApiConsumer<TKey, TValue>>());
    }

    private readonly IConsumerConfigurator _consumerConfigurator;
    private readonly IConsumerFactory _consumerFactory;
    private readonly IDeserializerFactory _deserializerFactory;
    private readonly ILoggerFactory _loggerFactory;
  }
}
