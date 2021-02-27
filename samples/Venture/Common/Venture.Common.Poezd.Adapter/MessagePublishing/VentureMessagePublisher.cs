#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Venture.Common.Application.MessagePublishing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessagePublishing
{
  /// <summary>
  /// The default message publisher for Venture product.
  /// </summary>
  public class VentureMessagePublisher : IMessagePublisher
  {
    public VentureMessagePublisher([NotNull] IMessageRouter messageRouter)
    {
      _messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
    }

    /// <inheritdoc />
    public Task Publish<TMessage>(TMessage message, VentureOutgoingMessageHandlingContext context) where TMessage : class
    {
      if (message == null) throw new ArgumentNullException(nameof(message));
      if (context == null) throw new ArgumentNullException(nameof(context));

      return _messageRouter.RouteOutgoingMessage(
        message,
        context.CorrelationId,
        context.CausationId,
        context.MessageId ?? Guid.NewGuid().ToString("N"));
    }

    private readonly IMessageRouter _messageRouter;
  }
}
