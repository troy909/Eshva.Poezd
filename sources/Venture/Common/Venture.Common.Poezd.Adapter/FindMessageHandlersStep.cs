#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  /// <summary>
  /// Finds all message handlers for a message stored as a POCO in the message handling context.
  /// </summary>
  public class FindMessageHandlersStep : IStep
  {
    public FindMessageHandlersStep([NotNull] IHandlerRegistry handlerRegistry, [NotNull] IServiceProvider serviceProvider)
    {
      _handlerRegistry = handlerRegistry ?? throw new ArgumentNullException(nameof(handlerRegistry));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var messageType = context.TakeOrThrow<Type>(ContextKeys.Application.MessageType);
      var handlers = GetHandlersForMessageType(messageType);
      context.Put(ContextKeys.Application.Handlers, handlers);
      return Task.CompletedTask;
    }

    private HandlerDescriptor[] GetHandlersForMessageType(Type messageType) =>
      _handlerRegistry.HandlersGroupedByMessageType[messageType]
        .Select(handlerType => _serviceProvider.GetService(handlerType))
        .Where(handler => handler != null)
        .Select(handler => MakeHandlerDescriptor(handler, messageType))
        .ToArray();

    private static HandlerDescriptor MakeHandlerDescriptor(object handler, Type messageType)
    {
      var handlerType = handler.GetType();
      var handleMethod = handlerType.GetMethod(
        nameof(IHandleMessageOfType<object>.Handle),
        new[] {messageType, typeof(VentureContext)});
      Func<object, VentureContext, Task> onHandle = (message, context) => (Task) handleMethod!.Invoke(handler, new[] {message, context});

      return new HandlerDescriptor(
        handlerType,
        messageType,
        onHandle);
    }

    private readonly IHandlerRegistry _handlerRegistry;
    private readonly IServiceProvider _serviceProvider;
  }
}
