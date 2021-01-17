#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.MessageHandling;
using Eshva.Poezd.SimpleInjectorCoupling;
using JetBrains.Annotations;
using SimpleInjector;
using Venture.Common.Application.MessageHandling;

#endregion


namespace Venture.Poezd.Coupling
{
  public sealed class VentureMessageHandlerFactory : MessageHandlerAdapter
  {
    public VentureMessageHandlerFactory([NotNull] Container container) : base(container, typeof(IHandleMessageOfType<>))
    {
    }

    protected override IHandleMessage CreatePoezdMessageHandler(object applicationHandler)
    {
      return new VentureMessageHandlerAdapter(applicationHandler);
    }

    private sealed class VentureMessageHandlerAdapter : IHandleMessage
    {
      public VentureMessageHandlerAdapter([NotNull] object applicationHandler)
      {
        // Get Handle method from handler
        _applicationHandler = applicationHandler ?? throw new ArgumentNullException(nameof(applicationHandler));
      }

      public Task Handle([NotNull] object message, [NotNull] IPocket poezdContext)
      {
        // ReSharper disable once CompareNonConstrainedGenericWithNull
        if (!message.GetType().IsValueType &&
            message == null) throw new ArgumentNullException(nameof(message));
        if (poezdContext == null) throw new ArgumentNullException(nameof(poezdContext));

        var applicationMessageHandler = AdoptPoezdMessageHandler(message);
        var applicationContext = AdoptPoezdMessageHandlingContext(poezdContext);
        return applicationMessageHandler(message, applicationContext);
      }

      private Func<object, VentureVentureMessageHandlingContext, Task> AdoptPoezdMessageHandler(object message)
      {
        // TODO: Reflection call is slow. I need to replace it with an Expression in the future.
        var messageType = message.GetType();
        var handlerConcreteType = typeof(IHandleMessageOfType<>)
                                  .GetGenericTypeDefinition()
                                  .MakeGenericType(messageType);
        var handleMethod = handlerConcreteType.GetMethod(nameof(IHandleMessageOfType<object>.Handle));
        if (handleMethod == null)
        {
          throw new InvalidOperationException(
            $"Method {nameof(IHandleMessageOfType<object>.Handle)} with arguments of types " +
            $"TMessage and {nameof(VentureVentureMessageHandlingContext)} isn't found on type {nameof(IHandleMessageOfType<object>)}");
        }

        return (message1, context) => (Task)handleMethod.Invoke(_applicationHandler, new[] { message1, context });
      }

      private static VentureVentureMessageHandlingContext AdoptPoezdMessageHandlingContext(IPocket context)
      {
        var ventureContext = new VentureVentureMessageHandlingContext();
        foreach (var (key, value) in context.GetItems())
        {
          ventureContext.Set(key, value);
        }

        return ventureContext;
      }

      private readonly object _applicationHandler;
    }
  }
}
