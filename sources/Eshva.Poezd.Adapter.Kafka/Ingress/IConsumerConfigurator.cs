#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  public interface IConsumerConfigurator
  {
    ConsumerBuilder<TKey, TValue> Configure<TKey, TValue>(
      ConsumerBuilder<TKey, TValue> builder,
      IDeserializer<TKey> keyDeserializer,
      IDeserializer<TValue> valueDeserializer);
  }
}
