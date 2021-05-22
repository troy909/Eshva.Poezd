#region Usings

using System;
using System.Collections.Generic;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// The ingress message handling context.
  /// </summary>
  public class MessageHandlingContext : ConcurrentPocket, ICanSkipFurtherMessageHandling
  {
    /// <summary>
    /// Gets/sets the broker message key.
    /// </summary>
    public object Key { get; set; }

    /// <summary>
    /// Gets/sets the broker message payload.
    /// </summary>
    public object Payload { get; set; }

    /// <summary>
    /// Gets/sets the broker message metadata/headers that was received with message.
    /// </summary>
    public IReadOnlyDictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Gets/sets the moment in time the message received.
    /// </summary>
    public DateTimeOffset ReceivedOnUtc { get; set; }

    /// <summary>
    /// Gets/sets the name of the queue/topic the message has been received from.
    /// </summary>
    public string QueueName { get; set; }

    /// <summary>
    /// Gets/sets the message broker the message received from.
    /// </summary>
    public IMessageBroker Broker { get; set; }

    /// <summary>
    /// Gets/sets the ingress API the broker message belongs to.
    /// </summary>
    public IIngressApi Api { get; set; }

    /// <summary>
    /// Gets/sets the message type name gotten from the broker message metadata/headers.
    /// </summary>
    public string TypeName { get; set; }

    /// <summary>
    /// Gets/sets the CLR-type of the parsed message.
    /// </summary>
    public Type MessageType { get; set; }

    /// <summary>
    /// Gets/sets the message type descriptor that used to parse the broker message payload.
    /// </summary>
    public object Descriptor { get; set; }

    /// <summary>
    /// Gets/sets the parsed message CLR-object.
    /// </summary>
    public object Message { get; set; }

    /// <summary>
    /// Gets/sets the correlation ID.
    /// </summary>
    public string CorrelationId { get; set; }

    /// <summary>
    /// Gets/sets the causation ID.
    /// </summary>
    public string CausationId { get; set; }

    /// <summary>
    /// Gets/sets the message ID.
    /// </summary>
    public string MessageId { get; set; }

    /// <summary>
    /// Gets/sets the list of message handlers that should be used to handle the message.
    /// </summary>
    /// <value>
    /// The type of the list and its items depends on application the library used in. You use this property in your
    /// pipeline steps to get and then to execute the message handlers.
    /// </value>
    public object Handlers { get; set; }

    /// <summary>
    /// Gets sign that this message should be skipped.
    /// </summary>
    public bool ShouldSkipFurtherMessageHandling { get; private set; }

    /// <summary>
    /// Sets sign that this message should be skipped.
    /// </summary>
    public void SkipFurtherMessageHandling() => ShouldSkipFurtherMessageHandling = true;
  }
}
