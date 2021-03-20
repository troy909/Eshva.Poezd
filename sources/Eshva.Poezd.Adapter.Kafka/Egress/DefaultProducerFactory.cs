#region Usings

using Confluent.Kafka;

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
      ISerializerFactory serializerFactory) =>
      configurator.Configure(
          new ProducerBuilder<TKey, TValue>(config),
          serializerFactory.Create<TKey>(),
          serializerFactory.Create<TValue>())
        .Build();
  }
}
