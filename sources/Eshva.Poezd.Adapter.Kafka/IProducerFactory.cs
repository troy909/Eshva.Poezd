#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public interface IProducerFactory
  {
    IProducer<TKey, TValue> Create<TKey, TValue>(ProducerConfig config);
  }
}
