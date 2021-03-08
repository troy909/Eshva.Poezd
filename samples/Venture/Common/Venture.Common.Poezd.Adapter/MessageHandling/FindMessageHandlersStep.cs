#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter.MessageHandling
{
  /// <summary>
  /// Finds all message handlers for a message object stored in a message handling context item.
  /// </summary>
  public class FindMessageHandlersStep : IStep<MessageHandlingContext>
  {
    public FindMessageHandlersStep([NotNull] IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public Task Execute(MessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));
      if (context.Api == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.Api));
      if (context.MessageType == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.MessageType));

      var handlerRegistry = context.Api.HandlerRegistry;
      var handlers = GetHandlersForMessageType(context.MessageType, handlerRegistry);
      context.Handlers = handlers;
      return Task.CompletedTask;
    }

    private HandlerDescriptor[] GetHandlersForMessageType(Type messageType, IHandlerRegistry handlerRegistry) =>
      handlerRegistry.HandlersGroupedByMessageType[messageType]
        .Select(handlerType => _serviceProvider.GetService(handlerType))
        .Where(handler => handler != null)
        .Select(handler => MakeHandlerDescriptor(handler, messageType))
        .ToArray();

    private static HandlerDescriptor MakeHandlerDescriptor(object handler, Type messageType)
    {
      var handlerType = handler.GetType();
      var handleMethod = handlerType.GetMethod(
        nameof(IMessageHandler<object>.Handle),
        new[] {messageType, typeof(VentureIncomingMessageHandlingContext)});
      Func<object, VentureIncomingMessageHandlingContext, Task> onHandle = (message, context) =>
        (Task) handleMethod!.Invoke(handler, new[] {message, context});

      return new HandlerDescriptor(
        handlerType,
        messageType,
        onHandle);
    }

    private readonly IServiceProvider _serviceProvider;
  }
}
