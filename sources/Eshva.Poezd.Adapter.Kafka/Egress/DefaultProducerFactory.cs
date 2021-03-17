#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  public class DefaultProducerFactory : IProducerFactory
  {
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
