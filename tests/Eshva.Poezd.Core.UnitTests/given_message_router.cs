#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using Eshva.Poezd.SimpleInjectorCoupling;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_message_router
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
    public async Task when_starting_with_invalid_configuration_it_should_fail()
    {
      var state = new TestDriverState();
      await using var container = RoutingTests
        .SetupContainer(_testOutputHelper)
        .AddRouterWithInvalidConfiguration(state);
      var messageRouter = container.GetMessageRouter();
      Func<Task> sut = () => messageRouter.Start();
      sut.Should().ThrowExactly<PoezdConfigurationException>()
        .Where(exception => exception.Message.StartsWith("Unable to start the message router due configuration errors:"));
    }

    [Fact]
    public async Task when_disposed_it_should_not_be_possible_to_start_router()
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
    public async Task when_constructed_without_required_arguments_it_should_fail()
    {
      var state = new TestDriverState();
      await using var container = RoutingTests
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
