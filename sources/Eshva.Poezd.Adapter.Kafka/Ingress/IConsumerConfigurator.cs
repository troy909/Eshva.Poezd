#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// Contract of consumer configurator.
  /// </summary>
  [PublicAPI]
  public interface IConsumerConfigurator
  {
    /// <summary>
    /// Configures consumer using <see cref="ConsumerBuilder{TKey,TValue}" /> methods.
    /// </summary>
    /// <typeparam name="TKey">
    /// The message key type.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The message payload type.
    /// </typeparam>
    /// <param name="builder">
    /// The consumer builder.
    /// </param>
    /// <param name="keyDeserializer">
    /// The message key deserializer.
    /// </param>
    /// <param name="valueDeserializer">
    /// The message payload deserializer.
    /// </param>
    /// <returns>
    /// Consumer builder.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// One of arguments not specified.
    /// </exception>
    [NotNull]
    ConsumerBuilder<TKey, TValue> Configure<TKey, TValue>(
      [NotNull] ConsumerBuilder<TKey, TValue> builder,
      [NotNull] IDeserializer<TKey> keyDeserializer,
      [NotNull] IDeserializer<TValue> valueDeserializer);
  }
}
