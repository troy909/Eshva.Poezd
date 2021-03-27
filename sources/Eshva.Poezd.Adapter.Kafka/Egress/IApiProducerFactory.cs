#region Usings

using System;
using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Contract of egress API producer factory.
  /// </summary>
  public interface IApiProducerFactory
  {
    /// <summary>
    /// Creates a new API producer with specified native producer configuration.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of message key.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of message payload.
    /// </typeparam>
    /// <param name="config">
    /// The native Kafka producer configuration.
    /// </param>
    /// <returns>
    /// The created API producer.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The native producer configuration is not specified.
    /// </exception>
    [NotNull]
    IApiProducer Create<TKey, TValue>([NotNull] ProducerConfig config);
  }
}
