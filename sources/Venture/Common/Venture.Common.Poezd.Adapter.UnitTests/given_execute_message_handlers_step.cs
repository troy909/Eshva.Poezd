#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Common.TestTools;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog.Sinks.InMemory;
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
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      var handler1 = new MessageHandler();
      var handler2 = new MessageHandler();
      var handler3 = new MessageHandler();
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        handler1,
        handler2,
        handler3);
      var context = VentureContextTools.CreateFilledContext(new Message02(), handlers);

      await sut.Execute(context);

      // check results
      handler1.IsExecuted.Should().BeTrue("handler #1 should be called");
      handler2.IsExecuted.Should().BeTrue("handler #2 should be called");
      handler3.IsExecuted.Should().BeTrue("handler #3 should be called");
    }

    [Fact]
    public void when_constructed_without_logger_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => new ExecuteMessageHandlersStep(
        logger: null,
        new ParallelHandlersExecutionPolicy(new NullLogger<ParallelHandlersExecutionPolicy>()));
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("logger"));
    }

    [Fact]
    public void when_constructed_without_handlers_execution_policy_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => new ExecuteMessageHandlersStep(
        new NullLogger<ExecuteMessageHandlersStep>(),
        executionPolicy: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("executionPolicy"));
    }

    [Fact]
    public void when_executed_without_required_context_items_it_should_throw()
    {
      var container = Logging.CreateContainerWithLogging();
      var step = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      IPocket context = null;
      // ReSharper disable once AccessToModifiedClosure - it's a test.
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

    [Fact]
    public void when_executed_with_3_handlers_it_should_execute_them_in_parallel()
    {
      var container = Logging.CreateContainerWithLogging();
      var step = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100)),
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100)),
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100)));
      var context = VentureContextTools.CreateFilledContext(new Message02(), handlers);

      Func<Task> sut = () => step.Execute(context);
      sut.ExecutionTime().Should().BeCloseTo(TimeSpan.FromMilliseconds(value: 100), TimeSpan.FromMilliseconds(value: 50));
    }

    [Fact]
    public async Task when_executed_it_should_log_start_end_finish_of_each_handler()
    {
      var container = Logging.CreateContainerWithLogging();
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        new MessageHandler(),
        new MessageHandler(),
        new MessageHandler());
      var context = VentureContextTools.CreateFilledContext(new Message02(), handlers);

      var sut = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      await sut.Execute(context);
      var logs = InMemorySink.Instance.LogEvents.Select(log => log.RenderMessage()).ToArray();
      logs.Should().Contain(log => log.StartsWith("Started to execute a message handler"));
      logs.Should().Contain(log => log.StartsWith("Started to execute a message handler"));
      logs.Should().Contain(log => log.StartsWith("Started to execute a message handler"));
      logs.Should().Contain(log => log.StartsWith("Finish to execute a message handler"));
      logs.Should().Contain(log => log.StartsWith("Finish to execute a message handler"));
      logs.Should().Contain(log => log.StartsWith("Finish to execute a message handler"));
    }

    [Fact]
    public async Task when_executed_and_one_of_handlers_throws_it_should_log_exception_but_other_should_be_successive()
    {
      var container = Logging.CreateContainerWithLogging();
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        new MessageHandler(),
        new MessageHandler(),
        new ThrowingHandler());
      var context = VentureContextTools.CreateFilledContext(new Message02(), handlers);

      var sut = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      await sut.Execute(context);
      var logs = InMemorySink.Instance.LogEvents.Select(log => log.RenderMessage()).ToArray();
      logs.Should().Contain(log => log.StartsWith("Started to execute a message handler"));
      logs.Should().Contain(log => log.StartsWith("Started to execute a message handler"));
      logs.Should().Contain(log => log.StartsWith("Started to execute a message handler"));
      logs.Count(log => log.StartsWith("Finish to execute a message handler")).Should().Be(expected: 2);
      logs.Should().Contain(log => log.StartsWith("Exception thrown during execution of message handler"));
    }
  }
}
