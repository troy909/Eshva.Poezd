#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// The default producer factory.
  /// </summary>
  /// <remarks>
  /// It configures it using configurator and uses serializer factory to create key and value serializers.
  /// </remarks>
  public sealed class DefaultProducerFactory : IProducerFactory
  {
    /// <summary>
    /// Creates a new instance of default producer factory.
    /// </summary>
    /// <param name="configurator">
    /// The producer configurator to configure different callbacks.
    /// </param>
    /// <param name="serializerFactory">
    /// The key and value serializers factory.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of parameters is not specified.
    /// </exception>
    public DefaultProducerFactory([NotNull] IProducerConfigurator configurator, [NotNull] ISerializerFactory serializerFactory)
    {
      _configurator = configurator ?? throw new ArgumentNullException(nameof(configurator));
      _serializerFactory = serializerFactory ?? throw new ArgumentNullException(nameof(serializerFactory));
    }

    /// <inheritdoc />
    public IProducer<TKey, TValue> Create<TKey, TValue>(ProducerConfig config)
    {
      if (config == null) throw new ArgumentNullException(nameof(config));

      var keySerializer = _serializerFactory.Create<TKey>() ??
                          throw new PoezdOperationException($"Can not create a serializer for key type {nameof(TKey)}.");
      var valueSerializer = _serializerFactory.Create<TValue>() ??
                            throw new PoezdOperationException($"Can not create a serializer for value type {nameof(TKey)}.");

      return _configurator.Configure(
          new ProducerBuilder<TKey, TValue>(config),
          keySerializer,
          valueSerializer)
        .Build();
    }

    private readonly IProducerConfigurator _configurator;
    private readonly ISerializerFactory _serializerFactory;
  }
}
