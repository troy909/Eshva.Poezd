#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  internal class DefaultProducerFactory : IProducerFactory
  {
    public IProducer<TKey, TValue> Create<TKey, TValue>(ProducerConfig config) => new ProducerBuilder<TKey, TValue>(config).Build();
  }
}
