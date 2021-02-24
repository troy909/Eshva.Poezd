#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Routing;
using Venture.Common.Application.MessageHandling;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public static class VentureContextTools
  {
    public static IEnumerable<HandlerDescriptor> CreateHandlerDescriptors(params IHandleMessageOfType<Message02>[] handlers) =>
      handlers.Select(
        handler => new HandlerDescriptor(
          handler.GetType(),
          typeof(Message02),
          (message, ventureContext) => handler.Handle((Message02) message, ventureContext)));

    public static IPocket CreateContextWithout(string itemKey)
    {
      var context = CreateFilledContext(new Message02(), new HandlerDescriptor[0]);
      context.TryRemove(itemKey);
      return context;
    }

    public static VentureContext CreateFilledContext(object message, IEnumerable<HandlerDescriptor> handlers)
    {
      var context = new VentureContext();
      context
        .Put(ContextKeys.Application.MessagePayload, message)
        .Put(ContextKeys.Application.MessageType, message.GetType())
        .Put(ContextKeys.Application.MessageId, Guid.NewGuid().ToString("N"))
        .Put(ContextKeys.Application.CorrelationId, Guid.NewGuid().ToString("N"))
        .Put(ContextKeys.Application.CausationId, Guid.NewGuid().ToString("N"))
        .Put(ContextKeys.Broker.QueueName, "case.facts.tasks.v1")
        .Put(ContextKeys.Application.Handlers, handlers)
        .Put(ContextKeys.Broker.ReceivedOnUtc, DateTimeOffset.UtcNow);

      return context;
    }
  }
}
