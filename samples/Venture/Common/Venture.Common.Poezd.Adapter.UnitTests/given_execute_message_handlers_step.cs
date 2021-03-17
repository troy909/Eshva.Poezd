#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Venture.Common.Application.MessageHandling;
using Venture.Common.Poezd.Adapter.Ingress;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_execute_message_handlers_step
  {
    [Fact]
    public async Task when_executed_with_filled_context_it_should_execute_all_handlers()
    {
      var sut = new ExecuteMessageHandlersStep(new ExecutionStrategy());

      var handler1 = new MessageHandler();
      var handler2 = new MessageHandler();
      var handler3 = new MessageHandler();
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        handler1,
        handler2,
        handler3);
      var context = VentureContextTools.CreateFilledPoezdContext(new Message02(), handlers);

      await sut.Execute(context);

      handler1.IsExecuted.Should().BeTrue("handler #1 should be called");
      handler2.IsExecuted.Should().BeTrue("handler #2 should be called");
      handler3.IsExecuted.Should().BeTrue("handler #3 should be called");
    }

    [Fact]
    public void when_constructed_without_handlers_execution_policy_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new ExecuteMessageHandlersStep(executionStrategy: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("executionStrategy"));
    }

    [Fact]
    public void when_executed_without_required_context_items_it_should_throw()
    {
      var step = new ExecuteMessageHandlersStep(new ExecutionStrategy());

      MessageHandlingContext context = null;
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once AccessToModifiedClosure - it's a way to test.
      Func<Task> sut = () => step.Execute(context!);
      context = VentureContextTools.CreateContextWithout(handlingContext => handlingContext.MessageType = null);
      sut.Should().Throw<PoezdOperationException>("message type is required");
      context = VentureContextTools.CreateContextWithout(handlingContext => handlingContext.QueueName = null);
      sut.Should().Throw<PoezdOperationException>("queue name is required");
      context = VentureContextTools.CreateContextWithout(handlingContext => handlingContext.Handlers = null);
      sut.Should().Throw<PoezdOperationException>("handlers are required");
      context = VentureContextTools.CreateContextWithout(handlingContext => handlingContext.ReceivedOnUtc = DateTimeOffset.MinValue);
      sut.Should().Throw<PoezdOperationException>("received on moment is required");
      context = VentureContextTools.CreateContextWithout(handlingContext => handlingContext.CorrelationId = null);
      sut.Should().NotThrow<PoezdOperationException>("correlation ID is not required");
      context = VentureContextTools.CreateContextWithout(handlingContext => handlingContext.CausationId = null);
      sut.Should().NotThrow<PoezdOperationException>("causation ID is not required");
      context = VentureContextTools.CreateContextWithout(handlingContext => handlingContext.MessageId = null);
      sut.Should().NotThrow<PoezdOperationException>("message ID is not required");
    }

    private class ExecutionStrategy : IHandlersExecutionStrategy
    {
      public async Task ExecuteHandlers(
        IEnumerable<HandlerDescriptor> handlers,
        object message,
        VentureIncomingMessageHandlingContext context)
      {
        foreach (var handler in handlers)
        {
          await handler.OnHandle(message, context);
        }
      }
    }
  }
}
