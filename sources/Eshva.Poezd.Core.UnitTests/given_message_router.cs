#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using SimpleInjector;
using Xunit;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class given_message_router
  {
    [Fact]
    public void when_route_message_requested_it_should_router_message_to_all_application_handlers_of_message_from_container()
    {
      var container = new Container();
      var testProperties = new TestProperties();
      container.RegisterInstance(testProperties);
      container.Collection.Register(typeof(ICustomHandler<>), Assembly.GetExecutingAssembly());
      container.Verify();

      var poezdConfiguration = ConfigurePoezd(container);
      var router = poezdConfiguration.CreateMessageRouter(container);

      router.RouteIncomingMessage("TODO", "TODO", DateTimeOffset.UtcNow, new byte[0], new Dictionary<string, string>(), "TODO");
      router.RouteIncomingMessage("TODO", "TODO", DateTimeOffset.UtcNow, new byte[0], new Dictionary<string, string>(), "TODO");
      router.RouteIncomingMessage("TODO", "TODO", DateTimeOffset.UtcNow, new byte[0], new Dictionary<string, string>(), "TODO");
      /*
      router.RouteIncomingMessage("TODO", "TODO", DateTimeOffset.UtcNow, new CustomCommand1(), transactionContext, "TODO");
      router.RouteIncomingMessage("TODO", "TODO", DateTimeOffset.UtcNow, new CustomCommand2(), new MessageHandlingContext(), "TODO");
      router.RouteIncomingMessage("TODO", "TODO", DateTimeOffset.UtcNow, new CustomCommand3(), new MessageHandlingContext(), "TODO");
      */

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

    [Fact]
    public void when_route_message_requested_it_should_provide_adopted_transaction_context_to_application_message_handlers()
    {
      var container = new Container();
      var testProperties = new TestProperties();
      container.RegisterInstance(testProperties);
      container.Collection.Register(typeof(ICustomHandler<>), Assembly.GetExecutingAssembly());
      container.Verify();

      var poezdConfiguration = ConfigurePoezd(container);
      var router = poezdConfiguration.CreateMessageRouter(container);
      var transactionContext = new MessageHandlingContext();
      const string ExpectedProperty1Value = "value1";
      transactionContext.Set(CustomHandler1.Property1, ExpectedProperty1Value);

      router.RouteIncomingMessage("TODO", "TODO", DateTimeOffset.UtcNow, new byte[0], new Dictionary<string, string>(), "TODO");
      // router.RouteIncomingMessage(TODO, TODO, TODO, new CustomCommand1(), transactionContext, TODO);

      testProperties.Property1.Should()
                    .Be(ExpectedProperty1Value, $"{nameof(CustomHandler1)} set property in own execution context");
    }


    private static PoezdConfiguration ConfigurePoezd(Container container)
    {
      var configuration =
        MessageRouter.Configure(
          configurator => configurator
            .WithMessageHandling(
              messageHandling => messageHandling
                .WithMessageHandlersFactory(new CustomMessageHandlerFactory(container))));
      return configuration;
    }

  }
}
