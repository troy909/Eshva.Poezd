#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_message_router_publishing_egress_messages
  {
    public given_message_router_publishing_egress_messages(ITestOutputHelper testOutputHelper)
    {
      _testOutputHelper = testOutputHelper;
    }

    [Fact]
    public async Task when_publishing_message_it_should_publish_using_driver()
    {
      var state = new TestDriverState();
      await using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithConfiguredEgressApi(state);
      var messageRouter = container.GetMessageRouter();
      await messageRouter.Start();

      await messageRouter.RouteEgressMessage(new TestEgressMessage1());

      state.PublishedMessageCount.Should().Be(expected: 1, "message should be published using driver");
    }

    [Fact]
    public async Task when_message_published_it_should_create_pipeline_for_its_publishing()
    {
      var state = new TestDriverState();
      await using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithConfiguredEgressApi(state);
      var messageRouter = container.GetMessageRouter();
      await messageRouter.Start();

      await messageRouter.RouteEgressMessage(new TestEgressMessage1());

      InMemorySink.Instance.LogEvents
        .Where(@event => @event.Level == LogEventLevel.Information)
        .Select(@event => @event.MessageTemplate.Text)
        .Should().BeEquivalentTo(
          new[]
          {
            nameof(TestBrokerEgressEnterStep),
            nameof(TestBrokerEgressApiStep),
            nameof(TestBrokerEgressExitStep)
          },
          options => options.WithStrictOrdering(),
          "pipeline should be built with expected steps order");
    }

    [Fact]
    public async Task when_publishing_and_some_pipeline_step_throws_exception_it_should_fail()
    {
      var state = new TestDriverState();
      await using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithThrowingHandler(state, "broker-1");
      var messageRouter = container.GetMessageRouter();
      await messageRouter.Start();

      Func<Task> sut = () => messageRouter.RouteEgressMessage(new TestEgressMessage1());

      sut.Should().ThrowExactly<PoezdOperationException>()
        .Where(exception => exception.Message.Contains("message publishing"), "exception in a step should break publishing");
    }

    private readonly ITestOutputHelper _testOutputHelper;
  }
}
