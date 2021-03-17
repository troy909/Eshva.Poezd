#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  public interface IProducerConfigurator
  {
    ProducerBuilder<TKey, TValue> Configure<TKey, TValue>(
      ProducerBuilder<TKey, TValue> builder,
      ISerializer<TKey> keySerializer,
      ISerializer<TValue> valueSerializer);
  }
}
