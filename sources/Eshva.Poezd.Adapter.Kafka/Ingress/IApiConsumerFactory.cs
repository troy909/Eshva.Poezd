#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// Contract of ingress API consumer factory.
  /// </summary>
  public interface IApiConsumerFactory
  {
    /// <summary>
    /// Creates a new API consumer with specified native consumer configuration.
    /// </summary>
    /// <typeparam name="TKey">
    /// The type of message key.
    /// </typeparam>
    /// <typeparam name="TValue">
    /// The type of message payload.
    /// </typeparam>
    /// <param name="config">
    /// The native Kafka consumer configuration.
    /// </param>
    /// <param name="api">
    /// The ingress API for which consumer is created.
    /// </param>
    /// <returns>
    /// The created API consumer.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of argument is not specified.
    /// </exception>
    [NotNull]
    IApiConsumer<TKey, TValue> Create<TKey, TValue>([NotNull] ConsumerConfig config, [NotNull] IIngressApi api);
  }
}
