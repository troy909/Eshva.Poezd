#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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
  public class given_find_message_handlers_step
  {
    [Fact]
    public async Task when_executed_with_message_type_within_context_it_should_find_handlers_for_this_message_and_store_them_in_context()
    {
      var container = CreateContainerWithLogging();

      var handlersRegistry = new WorkPlannerHandlersRegistry();
      var handlers = handlersRegistry.HandlersGroupedByMessageType[typeof(TaskCreated)];
      foreach (var handler in handlers)
      {
        container.Register(handler);
      }

      var sut = new FindMessageHandlersStep(handlersRegistry, container);
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Application.MessageType, typeof(TaskCreated));

      await sut.Execute(context);

      var foundHandlers = context.TakeOrNull<IEnumerable<Func<object, VentureContext, Task>>>(ContextKeys.Application.Handlers);
      var taskCreatedHandler = foundHandlers.Single();
      taskCreatedHandler.Should().BeOfType<Func<object, VentureContext, Task>>("should find the handler for the message");
      await taskCreatedHandler(new TaskCreated(), new VentureContext());
      InMemorySink.Instance.LogEvents.Should().Contain(@event => @event.MessageTemplate.Text.Contains(nameof(TaskCreated)));
    }

    [Fact]
    public void when_constructed_without_handlers_registry_it_should_throw()
    {
      var container = CreateContainerWithLogging();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new FindMessageHandlersStep(handlerRegistry: null, container);
      sut.Should().Throw<ArgumentNullException>("handlersRegistry is required");
    }

    [Fact]
    public void when_constructed_without_service_provider_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new FindMessageHandlersStep(new WorkPlannerHandlersRegistry(), serviceProvider: null);
      sut.Should().Throw<ArgumentNullException>("handlersRegistry is required");
    }

    [Fact]
    public void when_executed_without_context_it_should_throw()
    {
      var container = CreateContainerWithLogging();
      var handlersRegistry = new WorkPlannerHandlersRegistry();
      var step = new FindMessageHandlersStep(handlersRegistry, container);

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => step.Execute(context: null);
      sut.Should().Throw<ArgumentNullException>("context is required");
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
  }
}
