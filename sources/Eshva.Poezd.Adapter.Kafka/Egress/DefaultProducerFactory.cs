#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// The default producer factory.
  /// </summary>
  public sealed class DefaultProducerFactory : IProducerFactory
  {
    /// <inheritdoc />
    public IProducer<TKey, TValue> Create<TKey, TValue>(
      ProducerConfig config,
      IProducerConfigurator configurator,
      ISerializerFactory serializerFactory)
    {
      if (config == null) throw new ArgumentNullException(nameof(config));
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));
      if (serializerFactory == null) throw new ArgumentNullException(nameof(serializerFactory));

      var keySerializer = serializerFactory.Create<TKey>() ??
                          throw new PoezdOperationException($"Can not create a serializer for key type {nameof(TKey)}.");
      var valueSerializer = serializerFactory.Create<TValue>() ??
                            throw new PoezdOperationException($"Can not create a serializer for value type {nameof(TKey)}.");

      return configurator.Configure(
          new ProducerBuilder<TKey, TValue>(config),
          keySerializer,
          valueSerializer)
        .Build();
    }
  }
}
