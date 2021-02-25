#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter
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
    public Task Publish<TMessage>(TMessage message, VentureContext context) where TMessage : class
    {
      if (message == null) throw new ArgumentNullException(nameof(message));
      if (context == null) throw new ArgumentNullException(nameof(context));

      return _messageRouter.RouteOutgoingMessage(
        message,
        context.TakeOrPut(VentureContext.Keys.MessageId, () => Guid.NewGuid().ToString("N")),
        context.TakeOrThrow<string>(VentureContext.Keys.CorrelationId),
        context.TakeOrThrow<string>(VentureContext.Keys.CausationId));
    }

    private readonly IMessageRouter _messageRouter;
  }
}
