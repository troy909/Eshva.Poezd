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
  public interface IMessageRouter : IDisposable
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
    /// Route an incoming message to message handlers. Used by broker drivers.
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
    Task RouteIngressMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata);

    /// <summary>
    /// Route an outgoing message to message brokers queues/topics.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The message type.
    /// </typeparam>
    /// <param name="message">
    /// The message to route.
    /// </param>
    /// <param name="messageId">
    /// The message ID that will be used in broker message headers.
    /// </param>
    /// <param name="correlationId">
    /// The correlation ID that will be used in broker message headers.
    /// </param>
    /// <param name="causationId">
    /// The causation ID that will be used in broker message headers.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting the message routing finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message is not specified.
    /// </exception>
    [NotNull]
    Task RouteEgressMessage<TMessage>(
      [NotNull] TMessage message,
      string correlationId = default,
      string causationId = default,
      string messageId = default)
      where TMessage : class;
  }
}
