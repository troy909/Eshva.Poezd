#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Common.TestTools;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.Application.MessageHandling;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_find_message_handlers_step
  {
    public given_find_message_handlers_step(ITestOutputHelper testOutput)
    {
      _testOutput = testOutput;
    }

    [Fact]
    public async Task when_executed_with_message_type_within_context_it_should_find_handlers_for_this_message_and_store_them_in_context()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput);

      var handlersRegistry = new VentureServiceHandlersRegistry(new[] {typeof(Message01Handler).Assembly});
      var handlers = handlersRegistry.HandlersGroupedByMessageType[typeof(Message01)];
      foreach (var handler in handlers)
      {
        container.Register(handler);
      }

      var sut = new FindMessageHandlersStep(handlersRegistry, container);
      var context = new ConcurrentPocket();
      context.Put(ContextKeys.Application.MessageType, typeof(Message01));

      await sut.Execute(context);

      var foundHandlers = context.TakeOrNull<IEnumerable<HandlerDescriptor>>(ContextKeys.Application.Handlers);
      var singleHandler = foundHandlers.Single();
      singleHandler.Should().BeOfType<HandlerDescriptor>("should find the handler for the message");
      await singleHandler.OnHandle(new Message01(), new VentureContext());
      InMemorySink.Instance.LogEvents.Should().Contain(@event => @event.MessageTemplate.Text.Contains(nameof(Message01)));
    }

    [Fact]
    public void when_constructed_without_handlers_registry_it_should_throw()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput); // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new FindMessageHandlersStep(handlerRegistry: null, container);
      sut.Should().Throw<ArgumentNullException>("handlersRegistry is required");
    }

    [Fact]
    public void when_constructed_without_service_provider_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new FindMessageHandlersStep(
        new VentureServiceHandlersRegistry(new[] {typeof(Message01Handler).Assembly}),
        serviceProvider: null);
      sut.Should().Throw<ArgumentNullException>("handlersRegistry is required");
    }

    [Fact]
    public void when_executed_without_context_it_should_throw()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput);
      var handlersRegistry = new VentureServiceHandlersRegistry(new[] {typeof(Message01Handler).Assembly});
      var step = new FindMessageHandlersStep(handlersRegistry, container);

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => step.Execute(context: null);
      sut.Should().Throw<ArgumentNullException>("context is required");
    }

    private readonly ITestOutputHelper _testOutput;
  }
}
