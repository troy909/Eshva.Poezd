#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// The contract of a producer factory.
  /// </summary>
  public interface IProducerFactory
  {
    /// <summary>
    /// Creates a producer with <typeparamref name="TKey" /> and <typeparamref name="TValue" />, configuration
    /// <paramref name="config" />. It configures it using <paramref name="configurator" /> and uses
    /// <paramref name="serializerFactory" /> to create key and value serializers.
    /// </summary>
    /// <typeparam name="TKey">
    /// The key type.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The value type.
    /// </typeparam>
    /// <param name="config">
    /// The producer configuration.
    /// </param>
    /// <param name="configurator">
    /// The producer configurator to configure different callbacks.
    /// </param>
    /// <param name="serializerFactory">
    /// The key and value serializers factory.
    /// </param>
    /// <returns>
    /// Configured Kafka producer.
    /// </returns>
    [NotNull]
    IProducer<TKey, TValue> Create<TKey, TValue>(
      [NotNull] ProducerConfig config,
      [NotNull] IProducerConfigurator configurator,
      [NotNull] ISerializerFactory serializerFactory);
  }
}
