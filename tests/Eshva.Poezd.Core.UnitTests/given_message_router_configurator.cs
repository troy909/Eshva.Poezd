#region Usings

using System;
using System.Linq;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_message_router_configurator
  {
    [Fact]
    public void when_message_broker_added_it_should_be_added_into_configuration()
    {
      var sut = new MessageRouterConfigurator();
      const string expectedId = "id";
      sut.AddMessageBroker(broker => broker.WithId(expectedId));
      sut.Configuration.Brokers.Single(broker => broker.Id.Equals(expectedId)).Should().NotBeNull("broker should be added");
    }

    [Fact]
    public void when_null_added_as_message_broker_it_should_fail()
    {
      var configurator = new MessageRouterConfigurator();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.AddMessageBroker(configurator: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_configure_ingress_and_call_without_ingress_it_should_fail()
    {
      Action sut1 = () =>
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("broker id")
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(new TestDriverState())
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("ingress-api")
                        .WithHandlerRegistry<EmptyHandlerRegistry>()
                        .WithMessageKey<string>()
                        .WithMessagePayload<string>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithQueueNamePatternsProvider<ProvidingNothingQueueNamePatternsProvider>()
                    ))
                .WithoutIngress()
                .WithoutEgress()));

      sut1.Should().ThrowExactly<PoezdConfigurationException>().Which.Message.Should().Contain("WithoutIngress");

      Action sut2 = () =>
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("broker id")
                .WithoutIngress()
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(new TestDriverState())
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("ingress-api")
                        .WithHandlerRegistry<EmptyHandlerRegistry>()
                        .WithMessageKey<string>()
                        .WithMessagePayload<string>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithQueueNamePatternsProvider<ProvidingNothingQueueNamePatternsProvider>()
                    ))
                .WithoutEgress()));

      sut2.Should().ThrowExactly<PoezdConfigurationException>().Which.Message.Should().Contain("WithoutIngress");
    }

    [Fact]
    public void when_configure_egress_and_call_without_egress_it_should_fail()
    {
      Action sut1 = () =>
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("broker id")
                .WithoutIngress()
                .Egress(
                  egress => egress
                    .WithTestDriver(new TestDriverState())
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .AddApi(
                      api =>
                        api
                          .WithId("egress-api")
                          .WithMessageKey<string>()
                          .WithMessagePayload<string>()
                          .WithMessageTypesRegistry<EmptyEgressApiMessageTypesRegistry>()
                          .WithPipeFitter<EmptyPipeFitter>()
                    ))
                .WithoutEgress()));

      sut1.Should().ThrowExactly<PoezdConfigurationException>().Which.Message.Should().Contain("WithoutEgress");

      Action sut2 = () =>
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("broker id")
                .WithoutIngress()
                .WithoutEgress()
                .Egress(
                  egress => egress
                    .WithTestDriver(new TestDriverState())
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .AddApi(
                      api =>
                        api
                          .WithId("egress-api")
                          .WithMessageKey<string>()
                          .WithMessagePayload<string>()
                          .WithMessageTypesRegistry<EmptyEgressApiMessageTypesRegistry>()
                          .WithPipeFitter<EmptyPipeFitter>()
                    ))));

      sut2.Should().ThrowExactly<PoezdConfigurationException>().Which.Message.Should().Contain("WithoutEgress");
    }
  }
}
