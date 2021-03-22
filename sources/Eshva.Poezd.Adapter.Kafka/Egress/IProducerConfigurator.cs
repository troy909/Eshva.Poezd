#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Contract of producer configurator.
  /// </summary>
  [PublicAPI]
  public interface IProducerConfigurator
  {
    /// <summary>
    /// Configures producer using <see cref="ProducerBuilder{TKey,TValue}" /> methods.
    /// </summary>
    /// <typeparam name="TKey">
    /// The message key type.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The message payload type.
    /// </typeparam>
    /// <param name="builder">
    /// The producer builder.
    /// </param>
    /// <param name="keySerializer">
    /// The message key serializer.
    /// </param>
    /// <param name="valueSerializer">
    /// The message payload serializer.
    /// </param>
    /// <returns>
    /// Producer builder.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// One of arguments not specified.
    /// </exception>
    [NotNull]
    ProducerBuilder<TKey, TValue> Configure<TKey, TValue>(
      [NotNull] ProducerBuilder<TKey, TValue> builder,
      [NotNull] ISerializer<TKey> keySerializer,
      [NotNull] ISerializer<TValue> valueSerializer);
  }
}
