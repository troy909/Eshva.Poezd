#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.TestTools;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_parallel_handlers_execution_strategy
  {
    public given_parallel_handlers_execution_strategy(ITestOutputHelper testOutput)
    {
      _testOutput = testOutput;
    }

    [Fact]
    public async Task when_executed_with_few_handlers_it_should_execute_all_of_them()
    {
      var handler1 = new MessageHandler();
      var handler2 = new MessageHandler();
      var handler3 = new MessageHandler();
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        handler1,
        handler2,
        handler3).ToArray();
      var message = new Message02();
      var context = VentureContextTools.CreateFilledContext(message, handlers);
      var sut = new ParallelHandlersExecutionStrategy(NullLogger<ParallelHandlersExecutionStrategy>.Instance);

      await sut.ExecuteHandlers(
        handlers,
        message,
        context);

      handler1.IsExecuted.Should().BeTrue("handler #1 should be called");
      handler2.IsExecuted.Should().BeTrue("handler #2 should be called");
      handler3.IsExecuted.Should().BeTrue("handler #3 should be called");
    }

    [Fact]
    public void when_executed_with_few_handlers_it_should_execute_them_in_parallel()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput);
      var strategy = new ParallelHandlersExecutionStrategy(container.GetInstance<ILogger<ParallelHandlersExecutionStrategy>>());
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100)),
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100)),
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100)),
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100)),
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100)),
        new DelayedMessageHandler(TimeSpan.FromMilliseconds(value: 100))).ToArray();
      var message = new Message02();
      var context = VentureContextTools.CreateFilledContext(message, handlers);

      Func<Task> sut = () => strategy.ExecuteHandlers(
        handlers,
        message,
        context);
      sut.ExecutionTime().Should()
        .BeCloseTo(
          TimeSpan.FromMilliseconds(value: 100),
          TimeSpan.FromMilliseconds(value: 50),
          "should run simultaneously");
    }

    [Fact]
    public void when_executed_with_failing_handler_it_should_throw()
    {
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        new MessageHandler(),
        new ThrowingHandler(),
        new ThrowingHandler()).ToArray();
      var message = new Message02();
      var context = VentureContextTools.CreateFilledContext(message, handlers);
      var strategy = new ParallelHandlersExecutionStrategy(NullLogger<ParallelHandlersExecutionStrategy>.Instance);

      Func<Task> sut = () => strategy.ExecuteHandlers(
        handlers,
        message,
        context);

      sut.Should().ThrowExactly<AggregateException>()
        .Where(exception => exception.InnerExceptions.Count == 2, "two handlers did throw an exception");
    }

    [Fact]
    public void when_executed_with_handlers_it_should_log_execution_progress()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput);
      var handlers = VentureContextTools.CreateHandlerDescriptors(
        new MessageHandler(),
        new ThrowingHandler(),
        new ThrowingHandler()).ToArray();
      var message = new Message02();
      var context = VentureContextTools.CreateFilledContext(message, handlers);
      var strategy = new ParallelHandlersExecutionStrategy(container.GetInstance<ILogger<ParallelHandlersExecutionStrategy>>());

      Func<Task> sut = () => strategy.ExecuteHandlers(
        handlers,
        message,
        context);

      sut.Should().ThrowExactly<AggregateException>();
      InMemorySink.Instance.LogEvents
        .Where(log => log.Level == LogEventLevel.Debug)
        .Select(log => log.RenderMessage())
        .Count(log => log.StartsWith("Executing a message handler of type"))
        .Should().Be(handlers.Length, "for each handler should be logged its start");
      InMemorySink.Instance.LogEvents
        .Where(log => log.Level == LogEventLevel.Debug)
        .Select(log => log.RenderMessage())
        .Count(log => log.StartsWith("Executed a message handler of type"))
        .Should().Be(handlers.Length, "for each handler should be logged its finish");
      InMemorySink.Instance.LogEvents
        .Where(log => log.Level == LogEventLevel.Error)
        .Select(log => log.RenderMessage())
        .Count(log => log.StartsWith("An error occurred during a message handler execution of type"))
        .Should().Be(expected: 2, "for each failed handler should be logged an error");
    }

    [Fact]
    public void when_executed_without_handlers_it_should_throw()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput);
      var strategy = new ParallelHandlersExecutionStrategy(container.GetInstance<ILogger<ParallelHandlersExecutionStrategy>>());
      var handlers = VentureContextTools.CreateHandlerDescriptors(new MessageHandler()).ToArray();
      var message = new Message02();
      var context = VentureContextTools.CreateFilledContext(message, handlers);

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => strategy.ExecuteHandlers(
        handlers: null,
        message,
        context);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("handlers"));
    }

    [Fact]
    public void when_executed_without_message_it_should_throw()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput);
      var strategy = new ParallelHandlersExecutionStrategy(container.GetInstance<ILogger<ParallelHandlersExecutionStrategy>>());
      var handlers = VentureContextTools.CreateHandlerDescriptors(new MessageHandler()).ToArray();
      var message = new Message02();
      var context = VentureContextTools.CreateFilledContext(message, handlers);

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => strategy.ExecuteHandlers(
        handlers,
        message: null,
        context);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("message"));
    }

    [Fact]
    public void when_executed_without_context_it_should_throw()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput);
      var strategy = new ParallelHandlersExecutionStrategy(container.GetInstance<ILogger<ParallelHandlersExecutionStrategy>>());
      var handlers = VentureContextTools.CreateHandlerDescriptors(new MessageHandler()).ToArray();
      var message = new Message02();

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => strategy.ExecuteHandlers(
        handlers,
        message,
        context: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"));
    }

    [Fact]
    public void when_constructed_without_logger_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new ParallelHandlersExecutionStrategy(logger: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("logger"));
    }

    private readonly ITestOutputHelper _testOutput;
  }
}
