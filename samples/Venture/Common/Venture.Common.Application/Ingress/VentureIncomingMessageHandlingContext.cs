#region Usings

using System;
using Eshva.Common.Collections;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.MessageHandling
{
  /// <summary>
  /// Incoming message handling context fot the Venture sample product.
  /// </summary>
  /// <remarks>
  /// This class demonstrates using of a custom message handling context in an application. The application has no
  /// requirement to use the context class of Poezd library, effectively decouples application layer from the library which
  /// operates in the infrastructure layer. Your product can use any context class defined in your application layer or
  /// doesn't any at all being satisfied with message object itself.
  /// In this case message handlers in their <see cref="IMessageHandler{TMessage}.Handle" /> don't have a message object
  /// parameter. They take it from the message handling context which is the only parameter for this method.
  /// You should map the context items from library's context to this context in some message handling step in the adapter
  /// project. In the case of the Venture product it happens in the step
  /// Venture.CaseOffice.WorkPlanner.Adapter.ExecuteMessageHandlersStep class.
  /// As the base class for this context we use <see cref="ConcurrentPocket" /> that is a generic collection-like class but
  /// you  can use anything else you thing is better or don't use in your message handler a message handling context at all.
  /// Your application message handling steps can pass values to each other by adding context items into this context and get
  /// them when need them.
  /// </remarks>
  [UsedImplicitly(ImplicitUseTargetFlags.Members)]
  public class VentureIncomingMessageHandlingContext : ConcurrentPocket
  {
    public VentureIncomingMessageHandlingContext(
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
