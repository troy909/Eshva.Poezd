#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using Venture.Common.Application.MessageHandling;
using Venture.WorkPlanner.Messages.V1.Events;
using Xunit;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter.UnitTests
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class given_execute_message_handlers_step
  {
    [Fact]
    public async Task when_executed_with_filled_context_it_should_execute_all_handlers_in_it()
    {
      var container = CreateContainerWithLogging();
      var sut = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      var handler1 = new MessageHandler();
      var handler2 = new MessageHandler();
      var handler3 = new MessageHandler();
      var handlers = new Func<object, VentureContext, Task>[]
      {
        (message, ventureContext) => handler1.Handle((TaskCreated) message, ventureContext),
        (message, ventureContext) => handler2.Handle((TaskCreated) message, ventureContext),
        (message, ventureContext) => handler3.Handle((TaskCreated) message, ventureContext)
      };

      var context = CreateFilledContext(new TaskCreated(), handlers);

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
        logger: new NullLogger<ExecuteMessageHandlersStep>(),
        null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("logger"));
    }

    [Fact]
    public void when_executed_without_required_context_items_it_should_throw()
    {
      var container = CreateContainerWithLogging();
      var step = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      IPocket context = null;
      // ReSharper disable once AccessToModifiedClosure - it's a test.
      Func<Task> sut = () => step.Execute(context!);
      context = CreateContextWithout(ContextKeys.Application.MessageType);
      sut.Should().Throw<PoezdOperationException>();
      context = CreateContextWithout(ContextKeys.Broker.QueueName);
      sut.Should().Throw<PoezdOperationException>();
      context = CreateContextWithout(ContextKeys.Application.Handlers);
      sut.Should().Throw<PoezdOperationException>();
      context = CreateContextWithout(ContextKeys.Application.MessageId);
      sut.Should().NotThrow<PoezdOperationException>();
      context = CreateContextWithout(ContextKeys.Application.CorrelationId);
      sut.Should().NotThrow<PoezdOperationException>();
      context = CreateContextWithout(ContextKeys.Application.CausationId);
      sut.Should().NotThrow<PoezdOperationException>();
      context = CreateContextWithout(ContextKeys.Broker.ReceivedOnUtc);
      sut.Should().NotThrow<PoezdOperationException>();
    }

    [Fact]
    public void when_executed_with_3_handlers_it_should_execute_them_in_parallel()
    {
      var container = CreateContainerWithLogging();
      var step = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      var handler1 = new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100));
      var handler2 = new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 200));
      var handler3 = new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 300));
      var handlers = new Func<object, VentureContext, Task>[]
      {
        (message, ventureContext) => handler1.Handle((TaskCreated) message, ventureContext),
        (message, ventureContext) => handler2.Handle((TaskCreated) message, ventureContext),
        (message, ventureContext) => handler3.Handle((TaskCreated) message, ventureContext)
      };

      var context = CreateFilledContext(new TaskCreated(), handlers);

      Func<Task> sut = () => step.Execute(context);
      sut.ExecutionTime().Should().BeCloseTo(TimeSpan.FromMilliseconds(value: 300), TimeSpan.FromMilliseconds(value: 50));
    }

    [Fact]
    public async Task when_executed_it_should_log_start_end_finish_of_each_handler()
    {
      var container = CreateContainerWithLogging();

      var handler1 = new MessageHandler();
      var handler2 = new MessageHandler();
      var handler3 = new MessageHandler();
      var handlers = new Func<object, VentureContext, Task>[]
      {
        (message, ventureContext) => handler1.Handle((TaskCreated) message, ventureContext),
        (message, ventureContext) => handler2.Handle((TaskCreated) message, ventureContext),
        (message, ventureContext) => handler3.Handle((TaskCreated) message, ventureContext)
      };

      var context = CreateFilledContext(new TaskCreated(), handlers);

      var sut = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      await sut.Execute(context);
      var logs = InMemorySink.Instance.LogEvents.Select(log => log.RenderMessage()).ToArray();
      logs.Should().Contain(log => log.Equals("Started to execute a message handler #1."));
      logs.Should().Contain(log => log.Equals("Started to execute a message handler #2."));
      logs.Should().Contain(log => log.Equals("Started to execute a message handler #3."));
      logs.Should().Contain(log => log.StartsWith("Finish to execute a message handler #1 in "));
      logs.Should().Contain(log => log.StartsWith("Finish to execute a message handler #2 in "));
      logs.Should().Contain(log => log.StartsWith("Finish to execute a message handler #3 in "));
    }

    [Fact]
    public async Task when_executed_and_one_of_handlers_throws_it_should_log_exception_but_other_should_be_successive()
    {
      var container = CreateContainerWithLogging();

      var handler1 = new MessageHandler();
      var handler2 = new MessageHandler();
      var handler3 = new ThrowingHandler();
      var handlers = new Func<object, VentureContext, Task>[]
      {
        (message, ventureContext) => handler1.Handle((TaskCreated) message, ventureContext),
        (message, ventureContext) => handler2.Handle((TaskCreated) message, ventureContext),
        (message, ventureContext) => handler3.Handle((TaskCreated) message, ventureContext)
      };

      var context = CreateFilledContext(new TaskCreated(), handlers);

      var sut = new ExecuteMessageHandlersStep(
        container.GetInstance<ILogger<ExecuteMessageHandlersStep>>(),
        new ParallelHandlersExecutionPolicy(container.GetInstance<ILogger<ParallelHandlersExecutionPolicy>>()));

      await sut.Execute(context);
      var logs = InMemorySink.Instance.LogEvents.Select(log => log.RenderMessage()).ToArray();
      logs.Should().Contain(log => log.Equals("Started to execute a message handler #1."));
      logs.Should().Contain(log => log.Equals("Started to execute a message handler #2."));
      logs.Should().Contain(log => log.Equals("Started to execute a message handler #3."));
      logs.Count(log => log.StartsWith("Finish to execute a message handler #")).Should().Be(expected: 2);
      logs.Should().Contain(log => log.StartsWith("Exception thrown during execution of message handler #"));
    }


    private IPocket CreateContextWithout(string itemKey)
    {
      var context = CreateFilledContext(new TaskCreated(), new Func<object, VentureContext, Task>[0]);
      context.TryRemove(itemKey);
      return context;
    }


    private IPocket CreateFilledContext(object message, IEnumerable<Func<object, VentureContext, Task>> handlers)
    {
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Application.MessagePayload, message)
        .Put(ContextKeys.Application.MessageType, message.GetType())
        .Put(ContextKeys.Application.MessageId, Guid.NewGuid().ToString("N"))
        .Put(ContextKeys.Application.CorrelationId, Guid.NewGuid().ToString("N"))
        .Put(ContextKeys.Application.CausationId, Guid.NewGuid().ToString("N"))
        .Put(ContextKeys.Broker.QueueName, "case.facts.tasks.v1")
        .Put(ContextKeys.Application.Handlers, handlers)
        .Put(ContextKeys.Broker.ReceivedOnUtc, DateTimeOffset.UtcNow);


      return context;
    }

    private static Container CreateContainerWithLogging()
    {
      var container = new Container();
      container.RegisterInstance(GetLoggerFactory());
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);
      return container;
    }

    private static ILoggerFactory GetLoggerFactory() =>
      new LoggerFactory().AddSerilog(
        new LoggerConfiguration()
          .WriteTo.InMemory()
          .MinimumLevel.Verbose()
          .CreateLogger());

    private class MessageHandler : IHandleMessageOfType<TaskCreated>
    {
      public bool IsExecuted { get; private set; }

      public Task Handle(TaskCreated message, VentureContext context)
      {
        IsExecuted = true;
        return Task.CompletedTask;
      }
    }

    private class ThrowingHandler : IHandleMessageOfType<TaskCreated>
    {
      public Task Handle(TaskCreated message, VentureContext context) => throw new Exception(TestFail);

      private const string TestFail = "test fail";
    }

    private class DelayedMessageHandler : IHandleMessageOfType<TaskCreated>
    {
      public DelayedMessageHandler(TimeSpan timeout)
      {
        _timeout = timeout;
      }

      public Task Handle(TaskCreated message, VentureContext context) => Task.Delay(_timeout);

      private readonly TimeSpan _timeout;
    }
  }
}
