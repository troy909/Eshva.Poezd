#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;
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
  public class FindMessageHandlersStep : IStep<IPocket>
  {
    public FindMessageHandlersStep([NotNull] IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var publicApi = context.TakeOrThrow<IPublicApi>(ContextKeys.PublicApi.Itself);
      var handlerRegistry = publicApi.HandlerRegistry;
      var messageType = context.TakeOrThrow<Type>(ContextKeys.Application.MessageType);
      var handlers = GetHandlersForMessageType(messageType, handlerRegistry);
      context.Put(ContextKeys.Application.Handlers, handlers);
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
      Func<object, VentureIncomingMessageHandlingContext, Task> onHandle = (message, context) => (Task) handleMethod!.Invoke(handler, new[] {message, context});

      return new HandlerDescriptor(
        handlerType,
        messageType,
        onHandle);
    }

    private readonly IServiceProvider _serviceProvider;
  }
}
