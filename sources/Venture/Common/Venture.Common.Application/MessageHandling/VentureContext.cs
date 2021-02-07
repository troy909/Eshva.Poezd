#region Usings

using System;
using Eshva.Common.Collections;

#endregion


namespace Venture.Common.Application.MessageHandling
{
  /// <summary>
  /// Message handling context fot the Venture sample product.
  /// </summary>
  /// <remarks>
  /// This class demonstrates using of a custom message handling context in an application. The application has no
  /// requirement to use the context class of Poezd library, effectively decouples application layer from the library which
  /// operates in the infrastructure layer. Your product can use any context class defined in your application layer or
  /// doesn't any at all being satisfied with message object itself.
  /// In this case message handlers in their <see cref="IHandleMessageOfType{TMessage}.Handle" /> don't have a message object
  /// parameter. They take it from the message handling context which is the only parameter for this method.
  /// You should map the context items from library's context to this context in some message handling step in the adapter
  /// project. In the case of the Venture product it happens in the step
  /// Venture.CaseOffice.WorkPlanner.Adapter.ExecuteMessageHandlersStep class.
  /// As the base class for this context we use <see cref="ConcurrentPocket" /> as a generic collection-like class but you
  /// can use anything else you thing is better or don't use in your message handler a message handling context at all.
  /// </remarks>
  public class VentureContext : ConcurrentPocket, IVentureContext
  {
    /// <inheritdoc />
    public void Commit()
    {
      // TODO: Реализовать подтверждение обработки сообщения.
    }

    /// <inheritdoc />
    public void Abort()
    {
      // TODO: Реализовать прерывание обработки сообщения.
    }

    /// <summary>
    /// The keys of the message handling context items.
    /// </summary>
    /// <remarks>
    /// Keep specify the item type because items stored in the context (pocket) as plain <see cref="object" />.
    /// </remarks>
    public static class Keys
    {
      /// <summary>
      /// The message object.
      /// </summary>
      /// <value>
      /// The value is of the message CLR-type.
      /// </value>
      public const string Message = "Message";

      /// <summary>
      /// The message ID.
      /// </summary>
      /// <value>
      /// The value is a <see cref="Guid" />.
      /// </value>
      public const string MessageId = "MessageId";

      /// <summary>
      /// The CLR-type of the message object.
      /// </summary>
      /// <value>
      /// The value is a <see cref="Type" />.
      /// </value>
      public const string MessageType = "MessageType";

      /// <summary>
      /// The topic name the message came from.
      /// </summary>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      public const string SourceTopic = "SourceTopic";

      /// <summary>
      /// The key of correlation ID of the first message in this message conversation.
      /// </summary>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      /// <remarks>
      /// Correlation ID allows to trace the entire conversation between communicating parties.
      /// </remarks>
      public const string CorrelationId = "CorrelationId";

      /// <summary>
      /// The key of the message ID that causes this one.
      /// </summary>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      /// <remarks>
      /// Causation ID allows to find the direct cause of this message and trace it in conversation between communicating
      /// parties.
      /// </remarks>
      public const string CausationId = "CausationId";

      /// <summary>
      /// The moment in time when this message was received.
      /// </summary>
      /// <value>
      /// The value is a <see cref="DateTimeOffset" />.
      /// </value>
      public const string ReceivedOnUtc = "SentOnUtc";
    }
  }
}
