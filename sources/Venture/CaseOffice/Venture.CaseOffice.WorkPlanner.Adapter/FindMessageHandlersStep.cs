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

namespace Venture.CaseOffice.WorkPlanner.Adapter
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
      var handlerTypes = _handlerRegistry.HandlersGroupedByMessageType[messageType];

      var handlers = handlerTypes
        .Select(handlerType => _serviceProvider.GetService(handlerType))
        .Where(handler => handler != null)
        .Select(handler => MakeHandlerDescriptor(handler, messageType));

      context.Put(ContextKeys.Application.Handlers, handlers);
      return Task.CompletedTask;
    }

    private static HandlerDescriptor MakeHandlerDescriptor(object handler, Type messageType)
    {
      var type = handler.GetType();
      var handleMethod = type.GetMethod(
        nameof(IHandleMessageOfType<object>.Handle),
        new[] {messageType, typeof(VentureContext)});
      Func<object, VentureContext, Task> onHandle = (message, context) => (Task) handleMethod!.Invoke(handler, new[] {message, context});

      Func<object, VentureContext, Task> onCommit = null;
      if (CanCommit(type))
      {
        var commitMethod = type.GetMethod(nameof(ICanCommit<object>.Commit), new[] {messageType, typeof(VentureContext)});
        onCommit = (message, context) => (Task) commitMethod!.Invoke(handler, new[] {message, context});
      }

      Func<object, VentureContext, Task> onCompensate = null;
      if (CanCompensate(type))
      {
        var compensateMethod = type.GetMethod(nameof(ICanCompensate<object>.Compensate), new[] {messageType, typeof(VentureContext)});
        onCompensate = (message, context) => (Task) compensateMethod!.Invoke(handler, new[] {message, context});
      }

      return new HandlerDescriptor(
        type,
        onHandle,
        onCommit,
        onCompensate);
    }

    private static bool CanCompensate(Type type) =>
      type.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICanCompensate<>));

    private static bool CanCommit(Type type) =>
      type.GetInterfaces().Any(@interface => @interface.IsGenericType && @interface.GetGenericTypeDefinition() == typeof(ICanCommit<>));

    private readonly IHandlerRegistry _handlerRegistry;
    private readonly IServiceProvider _serviceProvider;
  }
}
