#region Usings

using System;
using Confluent.Kafka;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Default API producer factory.
  /// </summary>
  public class DefaultApiProducerFactory : IApiProducerFactory
  {
    /// <summary>
    /// Creates a new instance API producer factory.
    /// </summary>
    /// <param name="producerFactory">
    /// The native producer factory.
    /// </param>
    /// <param name="headerValueCodec">
    /// The header value codec.
    /// </param>
    /// <param name="loggerFactory">
    /// Logger factory.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public DefaultApiProducerFactory(
      [NotNull] IProducerFactory producerFactory,
      [NotNull] IHeaderValueCodec headerValueCodec,
      [NotNull] ILoggerFactory loggerFactory)
    {
      _producerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
      _headerValueCodec = headerValueCodec ?? throw new ArgumentNullException(nameof(headerValueCodec));
      _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
    }

    /// <inheritdoc />
    public IApiProducer Create<TKey, TValue>(ProducerConfig config)
    {
      var producer = _producerFactory.Create<TKey, TValue>(config ?? throw new ArgumentNullException(nameof(config)));

      return new DefaultApiProducer<TKey, TValue>(
        producer,
        _headerValueCodec,
        _loggerFactory.CreateLogger<DefaultApiProducer<TKey, TValue>>());
    }

    private readonly IHeaderValueCodec _headerValueCodec;
    private readonly ILoggerFactory _loggerFactory;
    private readonly IProducerFactory _producerFactory;
  }
}
