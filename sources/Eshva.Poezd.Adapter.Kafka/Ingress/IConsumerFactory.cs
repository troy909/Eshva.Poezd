#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// The contract of a producer factory.
  /// </summary>
  public interface IConsumerFactory
  {
    /// <summary>
    /// Creates a consumer with <typeparamref name="TKey" /> and <typeparamref name="TValue" />, configuration
    /// <paramref name="config" />. It configures it using <paramref name="configurator" /> and uses
    /// <paramref name="deserializerFactory" /> to create key and value deserializers.
    /// </summary>
    /// <typeparam name="TKey">
    /// The key type.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The value type.
    /// </typeparam>
    /// <param name="config">
    /// The consumer configuration.
    /// </param>
    /// <param name="configurator">
    /// The consumer configurator to configure different callbacks.
    /// </param>
    /// <param name="deserializerFactory">
    /// The key and value deserializers factory.
    /// </param>
    /// <returns>
    /// Configured Kafka consumer.
    /// </returns>
    [NotNull]
    IConsumer<TKey, TValue> Create<TKey, TValue>(
      [NotNull] ConsumerConfig config,
      [NotNull] IConsumerConfigurator configurator,
      [NotNull] IDeserializerFactory deserializerFactory);
  }
}
