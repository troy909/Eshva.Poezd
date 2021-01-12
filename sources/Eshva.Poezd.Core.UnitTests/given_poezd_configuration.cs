#region Usings

using System.Reflection;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using SimpleInjector;
using Xunit;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class given_poezd_configuration
  {
    [Fact]
    public void when_set_message_handling_configuration_it_should_be_stored()
    {
      var container = new Container();
      var poezdConfiguration = ConfigurePoezd(container);

      poezdConfiguration.MessageHandling.MessageHandlersFactory.Should()
                        .NotBeNull("it's configured").And
                        .BeOfType<CustomMessageHandlerFactory>($"set factory of type {nameof(CustomMessageHandlerFactory)}");
    }

    [Fact]
    public void when_set_message_handling_configuration_router_should_provide_handlers()
    {
      var container = new Container();
      var testProperties = new TestProperties();
      container.RegisterInstance(testProperties);
      container.Collection.Register(typeof(ICustomHandler<>), Assembly.GetExecutingAssembly());
      container.Verify();
      var poezdConfiguration = ConfigurePoezd(container);
      var router = poezdConfiguration.BuildMessageRouter();

      router.RouteMessage(new CustomCommand1());
      router.RouteMessage(new CustomCommand2());
      router.RouteMessage(new CustomCommand3());

      testProperties.Handled1.Should()
                    .Be(2, $"there is 2 handlers of {nameof(CustomCommand1)}: {nameof(CustomHandler1)} and {nameof(CustomHandler2)}");
      testProperties.Handled2.Should()
                    .Be(
                      3,
                      $"there is 3 handlers of {nameof(CustomCommand2)}: {nameof(CustomHandler2)}, " +
                      $"{nameof(CustomHandler12)} and {nameof(CustomHandler23)}");
      testProperties.Handled3.Should()
                    .Be(1, $"there is 1 handlers of {nameof(CustomCommand3)}: {nameof(CustomHandler23)}");
    }

    private static PoezdConfiguration ConfigurePoezd(Container container)
    {
      var poezdConfiguration =
        PoezdConfiguration.Create(
          configurator => configurator
            .WithMessageHandling(
              messageHandling => messageHandling.WithMessageHandlersFactory(new CustomMessageHandlerFactory(container))));
      return poezdConfiguration;
    }
  }
}
