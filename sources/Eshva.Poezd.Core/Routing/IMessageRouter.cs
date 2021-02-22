#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Message router. The core part of Poezd.
  /// </summary>
  public interface IMessageRouter
  {
    /// <summary>
    /// A list of message brokers connected to the router.
    /// </summary>
    [NotNull]
    IReadOnlyCollection<MessageBroker> Brokers { get; }

    /// <summary>
    /// Starts message routing.
    /// </summary>
    /// <param name="cancellationToken">
    /// Cancellation token that can be used to finish message routing.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting the routing starting is finished.
    /// </returns>
    /// <exception cref="PoezdOperationException">
    /// The router is started already or some error occurred during the router start.
    /// </exception>
    [NotNull]
    Task Start(CancellationToken cancellationToken = default);

    /// <summary>
    /// Router incoming message to message handlers. Used by broker drivers.
    /// </summary>
    /// <param name="brokerId">
    /// The message broker ID from which the message is arrived.
    /// </param>
    /// <param name="queueName">
    /// The queue/topic name from which the message is arrived.
    /// </param>
    /// <param name="receivedOnUtc">
    /// The moment in time the message was received.
    /// TODO: Should be the original message timestamp?
    /// </param>
    /// <param name="payload">
    /// The message payload.
    /// </param>
    /// <param name="metadata">
    /// The message metadata.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting the message routing finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is null, an empty or a whitespace string.
    /// </exception>
    [NotNull]
    Task RouteIncomingMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata);
  }
}
