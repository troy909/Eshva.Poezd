#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Common.TestTools;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_execute_message_handlers_step
  {
    [Fact]
    public async Task when_executed_with_filled_context_it_should_execute_all_handlers_in_it()
    {
      var container = Logging.CreateContainerWithLogging();
      var sut = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionStrategy(container.GetInstance<ILogger<ParallelHandlersExecutionStrategy>>()));

      var handler1 = new MessageHandler();
      var handler2 = new MessageHandler();
      var handler3 = new MessageHandler();
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        handler1,
        handler2,
        handler3);
      var context = VentureContextTools.CreateFilledContext(new Message02(), handlers);

      await sut.Execute(context);

      handler1.IsExecuted.Should().BeTrue("handler #1 should be called");
      handler2.IsExecuted.Should().BeTrue("handler #2 should be called");
      handler3.IsExecuted.Should().BeTrue("handler #3 should be called");
    }

    [Fact]
    public void when_constructed_without_logger_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => new ExecuteMessageHandlersStep(
        logger: null,
        new ParallelHandlersExecutionStrategy(new NullLogger<ParallelHandlersExecutionStrategy>()));
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("logger"));
    }

    [Fact]
    public void when_constructed_without_handlers_execution_policy_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => new ExecuteMessageHandlersStep(
        new NullLogger<ExecuteMessageHandlersStep>(),
        executionStrategy: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("executionStrategy"));
    }

    [Fact]
    public void when_executed_without_required_context_items_it_should_throw()
    {
      var container = Logging.CreateContainerWithLogging();
      var step = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionStrategy(container.GetInstance<ILogger<ParallelHandlersExecutionStrategy>>()));

      IPocket context = null;
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => step.Execute(context!);
      context = VentureContextTools.CreateContextWithout(ContextKeys.Application.MessageType);
      sut.Should().Throw<PoezdOperationException>();
      context = VentureContextTools.CreateContextWithout(ContextKeys.Broker.QueueName);
      sut.Should().Throw<PoezdOperationException>();
      context = VentureContextTools.CreateContextWithout(ContextKeys.Application.Handlers);
      sut.Should().Throw<PoezdOperationException>();
      context = VentureContextTools.CreateContextWithout(ContextKeys.Application.MessageId);
      sut.Should().NotThrow<PoezdOperationException>();
      context = VentureContextTools.CreateContextWithout(ContextKeys.Application.CorrelationId);
      sut.Should().NotThrow<PoezdOperationException>();
      context = VentureContextTools.CreateContextWithout(ContextKeys.Application.CausationId);
      sut.Should().NotThrow<PoezdOperationException>();
      context = VentureContextTools.CreateContextWithout(ContextKeys.Broker.ReceivedOnUtc);
      sut.Should().NotThrow<PoezdOperationException>();
    }
  }
}
