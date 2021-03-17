#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.MessagePublishing
{
  public class VentureOutgoingMessageHandlingContext
  {
    /// <summary>
    /// Constructs a new instance of Venture outgoing message handling context.
    /// </summary>
    /// <param name="message">
    /// The message to publish.
    /// </param>
    /// <param name="producedOnUtc">
    /// The moment in time the message was produced.
    /// </param>
    /// <param name="correlationId">
    /// The correlation ID.
    /// </param>
    /// <param name="causationId">
    /// The causation ID.
    /// </param>
    /// <param name="messageId">
    /// The message ID.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of required arguments is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    public VentureOutgoingMessageHandlingContext(
      [NotNull] object message,
      DateTimeOffset producedOnUtc,
      [NotNull] string correlationId,
      [NotNull] string causationId,
      [NotNull] string messageId)
    {
      if (string.IsNullOrWhiteSpace(correlationId)) throw new ArgumentNullException(nameof(correlationId));
      if (string.IsNullOrWhiteSpace(causationId)) throw new ArgumentNullException(nameof(causationId));
      if (string.IsNullOrWhiteSpace(messageId)) throw new ArgumentNullException(nameof(messageId));

      Message = message ?? throw new ArgumentNullException(nameof(message));
      ProducedOnUtc = producedOnUtc;
      CorrelationId = correlationId;
      CausationId = causationId;
      MessageId = messageId;
    }

    /// <summary>
    /// The message object.
    /// </summary>
    public object Message { get; }

    /// <summary>
    /// The moment in time when this message was produced.
    /// </summary>
    /// <remarks>
    /// Don't use <see cref="DateTimeOffset.UtcNow" /> directly in your code. Define in the application layer some
    /// <c>IClock</c> contract and it's implementations to generate this timestamp.
    /// </remarks>
    public DateTimeOffset ProducedOnUtc { get; }

    /// <summary>
    /// The key of correlation ID of the first message in this message conversation.
    /// </summary>
    /// <remarks>
    /// Correlation ID allows to trace the entire conversation between communicating parties. In the inception/first message
    /// this ID should be the same as the message ID. In subsequent/caused messages it should be copied from preceded/causing
    /// message.
    /// </remarks>
    public string CorrelationId { get; }

    /// <summary>
    /// The key of the message ID that causes this one.
    /// </summary>
    /// <remarks>
    /// Causation ID allows to find the direct cause of this message and trace it in conversation between communicating
    /// parties. You should put in this attribute the ID of a message that preceded/causing this one.
    /// </remarks>
    public string CausationId { get; }

    /// <summary>
    /// The message ID.
    /// </summary>
    /// <remarks>
    /// Each message should have an unique message ID.
    /// </remarks>
    public string MessageId { get; }
  }
}
