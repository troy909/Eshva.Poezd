#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// The default consumer factory.
  /// </summary>
  public sealed class DefaultConsumerFactory : IConsumerFactory
  {
    /// <inheritdoc />
    public IConsumer<TKey, TValue> Create<TKey, TValue>(
      ConsumerConfig config,
      IConsumerConfigurator configurator,
      IDeserializerFactory deserializerFactory) =>
      configurator.Configure(
          new ConsumerBuilder<TKey, TValue>(config),
          deserializerFactory.Create<TKey>(),
          deserializerFactory.Create<TValue>())
        .Build();
  }
}
