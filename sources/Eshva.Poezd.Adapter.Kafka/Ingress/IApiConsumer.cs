#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// Contract of an API consumer encapsulating Kafka API consumer and a message consumer thread.
  /// </summary>
  /// <typeparam name="TKey">
  /// The message key type.
  /// </typeparam>
  /// <typeparam name="TValue">
  /// The message payload type.
  /// </typeparam>
  [PublicAPI]
  public interface IApiConsumer<TKey, TValue> : IDisposable
  {
    /// <summary>
    /// Starts message consumption.
    /// </summary>
    /// <param name="onMessageReceived">
    /// Broker ingress message handler.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token.
    /// </param>
    /// <returns>
    /// Task that can be used to wait the message consumption is finished.
    /// </returns>
    Task Start([NotNull] Func<ConsumeResult<TKey, TValue>, Task> onMessageReceived, CancellationToken cancellationToken);

    /// <summary>
    /// Stops the message consumption from this consumer.
    /// </summary>
    void Stop();
  }
}
