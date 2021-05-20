#region Usings

using System;
using System.Collections.Generic;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Message publishing context for publishing egress messages.
  /// </summary>
  /// <remarks>
  /// This class implements <see cref="IPocket" /> contract. That means you can use this context to pass any data between
  /// your pipeline steps using this interface.
  /// </remarks>
  public class MessagePublishingContext : ConcurrentPocket, ICanSkipFurtherMessageHandling
  {
    /// <summary>
    /// Gets/sets the publishing message CLR-object.
    /// </summary>
    public object Message { get; set; }

    /// <summary>
    /// Gets/sets the message broker CLR-object that will be used to publish the message.
    /// </summary>
    public IMessageBroker Broker { get; set; }

    /// <summary>
    /// Gets/sets the egress API CLR-object that will be rule the publishing process.
    /// </summary>
    public IEgressApi Api { get; set; }

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
    /// Gets/sets the message broker key of the publishing message.
    /// </summary>
    /// <remarks>
    /// For Kafka you should specify this key when publishing messages to be able to process messages with a few instances of
    /// an application. But not every message broker requires this key.
    /// </remarks>
    /// TODO: May be I should change the property type to string or byte array?
    public object Key { get; set; }

    /// <summary>
    /// Gets/sets the message payload as a byte array.
    /// </summary>
    // TODO: Can I change it to Memory<byte> or Span<byte>?
    public object Payload { get; set; }

    /// <summary>
    /// Gets/sets the message metadata that will be send along with the message.
    /// </summary>
    /// <remarks>
    /// Useful, for instance, for specifying the message type that will be used on the receiving side to parse the message
    /// payload.
    /// </remarks>
    public IReadOnlyDictionary<string, string> Metadata { get; set; }

    /// <summary>
    /// Gets/sets the queue/topic name the publishing message should be published to.
    /// </summary>
    public IReadOnlyCollection<string> QueueNames { get; set; }

    /// <summary>
    /// Gets/sets the message timestamp.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <inheritdoc />
    public bool ShouldSkipFurtherMessageHandling { get; private set; }

    /// <inheritdoc />
    public void SkipFurtherMessageHandling() => ShouldSkipFurtherMessageHandling = true;
  }
}
