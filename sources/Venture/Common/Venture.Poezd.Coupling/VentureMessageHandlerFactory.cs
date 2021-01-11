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

    protected override IHandleMessage<TMessage> CreatePoezdMessageHandler<TMessage>(object handler)
    {
      return new VentureMessageHandlerAdapter<TMessage>(handler);
    }

    private sealed class VentureMessageHandlerAdapter<TMessage> : IHandleMessage<TMessage>
    {
      public VentureMessageHandlerAdapter([CanBeNull] object ventureHandler)
      {
        _ventureHandler = (IHandleMessageOfType<TMessage>)ventureHandler ?? throw new ArgumentNullException(nameof(ventureHandler));
      }

      public Task Handle([NotNull] TMessage message, [NotNull] IPocket poezdContext)
      {
        // ReSharper disable once CompareNonConstrainedGenericWithNull
        if (!message.GetType().IsValueType &&
            message == null) throw new ArgumentNullException(nameof(message));
        if (poezdContext == null) throw new ArgumentNullException(nameof(poezdContext));

        return _ventureHandler.Handle(message, MakeVentureMessageHandlingContext(poezdContext));
      }

      private static VentureMessageHandlingContext MakeVentureMessageHandlingContext(IPocket context)
      {
        var ventureContext = new VentureMessageHandlingContext();
        foreach (var (key, value) in context.GetItems())
        {
          ventureContext.Set(key, value);
        }

        return ventureContext;
      }

      private readonly IHandleMessageOfType<TMessage> _ventureHandler;
    }
  }
}
