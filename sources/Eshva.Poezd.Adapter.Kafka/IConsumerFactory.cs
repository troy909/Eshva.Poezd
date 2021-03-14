#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public interface IConsumerFactory
  {
    IConsumer<TKey, TValue> Create<TKey, TValue>(
      ConsumerConfig consumerConfig,
      IConsumerConfigurator configurator,
      IDeserializerFactory deserializerFactory);
  }
}
