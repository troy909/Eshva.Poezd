#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class given_message_router_handling_ingress_messages
  {
    public given_message_router_handling_ingress_messages(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task when_starting_it_should_subscribe_to_all_queues_specified_in_configuration()
    {
      var state = new TestDriverState();
      await using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithComplexApis<SampleBrokerPipeFitter, EmptyPipeFitter, EmptyPipeFitter, EmptyPipeFitter>(state);
      var messageRouter = container.GetMessageRouter();
      await messageRouter.Start();

      var patters = state.SubscribedQueueNamePatters;
      patters.Should().BeEquivalentTo(
        new[]
        {
          @"^sample\.(commands|facts)\.service1\.v1",
          "sample.facts.service-2.v1",
          @"^sample\.cdc\..*"
        },
        "it is full list of subscribed queues");
    }

    [Fact]
    public async Task when_message_received_it_should_create_pipeline_for_its_handling()
    {
      var state = new TestDriverState();
      await using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithComplexApis<SampleBrokerPipeFitter, EmptyPipeFitter, EmptyPipeFitter, EmptyPipeFitter>(state);
      var messageRouter = container.GetMessageRouter();
      await messageRouter.Start();
      await messageRouter.RouteIngressMessage(
        RoutingTests.SampleBrokerServer,
        "sample.facts.service-2.v1",
        DateTimeOffset.UtcNow,
        new byte[0],
        new Dictionary<string, string>());

      InMemorySink.Instance.LogEvents
        .Where(@event => @event.Level == LogEventLevel.Information)
        .Select(@event => @event.MessageTemplate.Text)
        .Should().BeEquivalentTo(
          new[]
          {
            nameof(LogMessageHandlingContextStep),
            nameof(Service2DeserializeMessageStep),
            nameof(GetMessageHandlersStep),
            nameof(DispatchMessageToHandlersStep),
            nameof(CommitMessageStep)
          },
          options => options.WithStrictOrdering(),
          "pipeline should be built with expected steps order");
    }

    [Fact]
    public async Task when_message_handler_throws_any_exception_it_should_stop_ingress_message_routing()
    {
      var state = new TestDriverState();
      const string brokerName = "broker-1";
      const string queueName = "queue-1";
      await using var container = RoutingTests.SetupContainer(_testOutputHelper).AddRouterWithThrowingHandler(state, brokerName);
      var messageRouter = container.GetMessageRouter();
      await messageRouter.Start();
      Func<Task> sut = () => messageRouter.RouteIngressMessage(
        brokerName,
        queueName,
        DateTimeOffset.UtcNow,
        new byte[0],
        new Dictionary<string, string>());
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("message handling"),
        $"exception should be thrown by {nameof(MessageRouter.RouteIngressMessage)}");
      // TODO: It's a wrong behavior. What to do with erroneous message should decide some API-related strategy.
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("stopped"),
        "it's not possible to route a message after another message handled with an error");
    }

    [Fact]
    public async Task when_some_message_handling_step_request_to_break_pipeline_execution_it_should_be_allowed_to_handle_next_message()
    {
      var state = new TestDriverState();
      const string brokerName = "broker-1";
      const string queueName = "queue-1";
      await using var container = RoutingTests.SetupContainer(_testOutputHelper).AddRouterWithBreakHandlingStep(state, brokerName);
      var messageRouter = container.GetMessageRouter();
      await messageRouter.Start();
      Func<Task> sut = () => messageRouter.RouteIngressMessage(
        brokerName,
        queueName,
        DateTimeOffset.UtcNow,
        new byte[0],
        new Dictionary<string, string>());
      sut.Should().NotThrow("message just skipped");
      sut.Should().NotThrow("further message handling should not be stopped");
    }

    private readonly ITestOutputHelper _testOutputHelper;
  }
}
