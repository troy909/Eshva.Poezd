#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using Eshva.Poezd.SimpleInjectorCoupling;
using FluentAssertions;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class given_message_router
  {
    public given_message_router(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task when_started_it_should_list_all_brokers()
    {
      var broker1Name = RoutingTests.RandomString();
      var broker2Name = RoutingTests.RandomString();
      var broker3Name = RoutingTests.RandomString();
      var state = new TestDriverState();
      await using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithThreeBrokers(
          broker1Name,
          broker2Name,
          broker3Name,
          state);
      var messageRouter = container.GetMessageRouter();
      await messageRouter.Start();

      messageRouter.Brokers.Select(broker => broker.Id).Should().BeEquivalentTo(
        new[] {broker1Name, broker2Name, broker3Name},
        "all declared brokers should be listed");
    }

    [Fact]
    public async Task when_used_it_should_live_expected_life()
    {
      var state = new TestDriverState();
      await using (var container = RoutingTests.SetupContainer(_testOutputHelper)
        .AddRouterWithThreeBrokers(
          RoutingTests.RandomString(),
          RoutingTests.RandomString(),
          RoutingTests.RandomString(),
          state))
      {
        var messageRouter = container.GetMessageRouter();
        state.InitializedCount.Should().Be(expected: 0, "all 6 drivers should not be initialized");
        state.MessageConsumingStartedCount.Should().Be(expected: 0, "among 6 drivers none should start message consuming at this moment");
        await messageRouter.Start();
        state.MessageConsumingStartedCount.Should().Be(expected: 3, "among 6 drivers 3 ingress drivers should be started");
        state.DisposedCount.Should().Be(expected: 0, "among 6 drivers none should be disposed at this moment");
      }

      state.DisposedCount.Should().Be(expected: 6, "all 6 drivers should be disposed at this moment");
    }

    [Fact]
    public void when_starting_with_invalid_configuration_it_should_fail()
    {
      var state = new TestDriverState();
      using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithInvalidConfiguration(state);
      var messageRouter = container.GetMessageRouter();
      Func<Task> sut = () => messageRouter.Start();
      sut.Should().ThrowExactly<PoezdConfigurationException>()
        .Where(exception => exception.Message.StartsWith("Unable to start the message router due configuration errors:"));
    }

    [Fact]
    public async Task when_disposed_it_should_be_possible_start_router()
    {
      var state = new TestDriverState();
      IMessageRouter messageRouter;
      await using (var container = RoutingTests.SetupContainer(_testOutputHelper)
        .AddRouterWithThreeBrokers(
          RoutingTests.RandomString(),
          RoutingTests.RandomString(),
          RoutingTests.RandomString(),
          state))
      {
        messageRouter = container.GetMessageRouter();
      }

      Func<Task> sut = () => messageRouter.Start();
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("disposed", StringComparison.InvariantCultureIgnoreCase),
        "it's not possible to start disposed message router");
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
          options => options.WithStrictOrdering());
    }

    [Fact]
    public void when_constructed_without_required_arguments_it_should_fail()
    {
      var state = new TestDriverState();
      using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithComplexApis<SampleBrokerPipeFitter, EmptyPipeFitter, EmptyPipeFitter, EmptyPipeFitter>(state);

      var containerAdapter = new SimpleInjectorAdapter(container);
      var configuration = new MessageRouterConfiguration();
      // ReSharper disable AccessToModifiedClosure - it's a way to test.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new MessageRouter(configuration, containerAdapter);

      containerAdapter = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("diContainerAdapter"));

      containerAdapter = new SimpleInjectorAdapter(container);
      configuration = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("configuration"));
    }

    private readonly ITestOutputHelper _testOutputHelper;
  }
}
