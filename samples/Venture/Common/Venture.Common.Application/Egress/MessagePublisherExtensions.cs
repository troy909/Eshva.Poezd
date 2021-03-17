#region Usings

using System;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.MessagePublishing
{
  /// <summary>
  /// Extensions for <see cref="IMessagePublisher" />.
  /// </summary>
  public static class MessagePublisherExtensions
  {
    /// <summary>
    /// Publishes the inception message.
    /// </summary>
    /// <remarks>
    /// The inception message is the message that starts inter-process communications. This message has the same correlation,
    /// causation and message IDs. This ID will be generated using <see cref="MessageId" /> as string representation of a new
    /// <see cref="Guid" /> converted into <see cref="string" /> using 'digits' ("N") format.
    /// Don't use <see cref="DateTimeOffset.UtcNow" /> directly in your code. Define in the application layer some
    /// <c>IClock</c> contract and it's implementations to generate this timestamp.
    /// </remarks>
    /// <typeparam name="TMessage">
    /// The type of the publishing message.
    /// </typeparam>
    /// <param name="publisher">
    /// The publisher.
    /// </param>
    /// <param name="message">
    /// The publishing message.
    /// </param>
    /// <param name="generatedOnUtc">
    /// The moment in time the message was produced.
    /// </param>
    /// <returns>
    /// The task that could be used to wait when publish is finished.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// The message is not specified.
    /// </exception>
    public static Task PublishInceptionMessage<TMessage>(
      this IMessagePublisher publisher,
      [NotNull] TMessage message,
      DateTimeOffset generatedOnUtc)
      where TMessage : class
    {
      if (message == null) throw new ArgumentNullException(nameof(message));

      var messageId = MessageId.Generate();
      return publisher.Publish(
        message,
        new VentureOutgoingMessageHandlingContext(
          message,
          generatedOnUtc,
          messageId,
          messageId,
          messageId));
    }

    /// <summary>
    /// Publishes a subsequent message.
    /// </summary>
    /// <remarks>
    /// A subsequent message generates as a side-effect of another message or a synchronous request. This message should have a
    /// new message ID, the same correlation ID and causation ID equals the message ID of the preceding message or the
    /// triggering synchronous request ID. The message ID will be generated using <see cref="MessageId" /> as string
    /// representation of a new <see cref="Guid" /> converted into <see cref="string" /> using 'digits' ("N") format.
    /// Don't use <see cref="DateTimeOffset.UtcNow" /> directly in your code. Define in the application layer some
    /// <c>IClock</c> contract and it's implementations to generate this timestamp.
    /// </remarks>
    /// <typeparam name="TMessage">
    /// The type of the publishing message.
    /// </typeparam>
    /// <param name="publisher">
    /// The publisher.
    /// </param>
    /// <param name="message">
    /// The publishing message.
    /// </param>
    /// <param name="generatedOnUtc">
    /// The moment in time the message was produced.
    /// </param>
    /// <param name="correlationId">
    /// The correlation ID.
    /// </param>
    /// <param name="precedingMessageId">
    /// The preceding message ID.
    /// </param>
    /// <returns>
    /// The task that could be used to wait when publish is finished.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// One of arguments is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    public static Task PublishSubsequentMessage<TMessage>(
      this IMessagePublisher publisher,
      [NotNull] TMessage message,
      DateTimeOffset generatedOnUtc,
      [NotNull] string correlationId,
      [NotNull] string precedingMessageId)
      where TMessage : class
    {
      if (message == null) throw new ArgumentNullException(nameof(message));
      if (string.IsNullOrWhiteSpace(correlationId)) throw new ArgumentNullException(nameof(correlationId));
      if (string.IsNullOrWhiteSpace(precedingMessageId)) throw new ArgumentNullException(nameof(precedingMessageId));

      var messageId = MessageId.Generate();
      return publisher.Publish(
        message,
        new VentureOutgoingMessageHandlingContext(
          message,
          generatedOnUtc,
          correlationId,
          precedingMessageId,
          messageId));
    }
  }
}
