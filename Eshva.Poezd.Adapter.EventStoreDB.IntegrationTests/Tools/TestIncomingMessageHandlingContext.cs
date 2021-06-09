#region Usings

using System;
using Eshva.Common.Collections;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public class TestIncomingMessageHandlingContext : ConcurrentPocket
  {
    public TestIncomingMessageHandlingContext(
      [NotNull] object message,
      [NotNull] Type messageType,
      [NotNull] string sourceTopic,
      DateTimeOffset receivedOnUtc,
      [NotNull] string correlationId,
      [NotNull] string causationId,
      [NotNull] string messageId)
    {
      if (string.IsNullOrWhiteSpace(sourceTopic)) throw new ArgumentNullException(nameof(sourceTopic));
      if (string.IsNullOrWhiteSpace(correlationId)) throw new ArgumentNullException(nameof(correlationId));
      if (string.IsNullOrWhiteSpace(causationId)) throw new ArgumentNullException(nameof(causationId));
      if (string.IsNullOrWhiteSpace(messageId)) throw new ArgumentNullException(nameof(messageId));

      Message = message ?? throw new ArgumentNullException(nameof(message));
      MessageType = messageType ?? throw new ArgumentNullException(nameof(messageType));
      SourceTopic = sourceTopic;
      ReceivedOnUtc = receivedOnUtc;
      CorrelationId = correlationId;
      CausationId = causationId;
      MessageId = messageId;
    }

    /// <summary>
    /// The message object.
    /// </summary>
    public object Message { get; }

    /// <summary>
    /// The CLR-type of the message object.
    /// </summary>
    public Type MessageType { get; }

    /// <summary>
    /// The topic name the message came from.
    /// </summary>
    public string SourceTopic { get; }

    /// <summary>
    /// The moment in time when this message was received.
    /// </summary>
    public DateTimeOffset ReceivedOnUtc { get; }

    /// <summary>
    /// The key of correlation ID of the first message in this message conversation.
    /// </summary>
    /// <remarks>
    /// Correlation ID allows to trace the entire conversation between communicating parties.
    /// </remarks>
    public string CorrelationId { get; }

    /// <summary>
    /// The key of the message ID that causes this one.
    /// </summary>
    /// <remarks>
    /// Causation ID allows to find the direct cause of this message and trace it in conversation between communicating
    /// parties.
    /// </remarks>
    public string CausationId { get; }

    /// <summary>
    /// The message ID.
    /// </summary>
    public string MessageId { get; }
  }
}
