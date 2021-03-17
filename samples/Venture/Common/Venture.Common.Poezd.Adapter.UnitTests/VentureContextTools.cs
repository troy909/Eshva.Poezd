#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Routing;
using Venture.Common.Application.Ingress;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public static class VentureContextTools
  {
    public static IEnumerable<HandlerDescriptor> CreateHandlerDescriptors(params IMessageHandler<Message02>[] handlers) =>
      handlers.Select(
        handler => new HandlerDescriptor(
          handler.GetType(),
          typeof(Message02),
          (message, ventureContext) => handler.Handle((Message02) message, ventureContext)));

    public static MessageHandlingContext CreateContextWithout(Action<MessageHandlingContext> updater)
    {
      var context = CreateFilledPoezdContext(new Message02(), new HandlerDescriptor[0]);
      updater(context);
      return context;
    }

    public static MessageHandlingContext CreateFilledPoezdContext(object message, IEnumerable<HandlerDescriptor> handlers) =>
      new MessageHandlingContext
      {
        Message = message,
        MessageType = message.GetType(),
        MessageId = Guid.NewGuid().ToString("N"),
        CorrelationId = Guid.NewGuid().ToString("N"),
        CausationId = Guid.NewGuid().ToString("N"),
        QueueName = "case.facts.tasks.v1",
        Handlers = handlers,
        ReceivedOnUtc = DateTimeOffset.UtcNow
      };

    public static VentureIncomingMessageHandlingContext CreateFilledVentureContext(object message) =>
      new VentureIncomingMessageHandlingContext(
        message,
        message.GetType(),
        "case.facts.tasks.v1",
        DateTimeOffset.UtcNow,
        Guid.NewGuid().ToString("N"),
        Guid.NewGuid().ToString("N"),
        Guid.NewGuid().ToString("N"));
  }
}
