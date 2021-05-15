#region Usings

using System;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Adapter.SimpleInjector;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.IntegrationTests.Tools;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.IntegrationTests
{
  public class given_broker_configurator_with_di_container
  {
    [Fact]
    public void when_configure_ingress_and_egress_it_should_configure_router_expected_way_using_services_from_container()
    {
      var (routerConfiguration, _) = CreateRouterWithBothIngressAndEgress();
      var brokerConfiguration = routerConfiguration.Brokers.Single();
      brokerConfiguration.Egress.Should().NotBeNull("egress сконфигурирован и должен быть недоступен");
    }

    [Fact]
    public void when_configured_with_egress_and_publish_but_no_api_knows_about_message_type_it_should_fail()
    {
      var (_, router) = CreateRouterWithBothIngressAndEgress();
      Func<Task> sut = () => router.RouteEgressMessage(new object());
      sut.Should().ThrowExactly<PoezdOperationException>("egress сконфигурирован, но ни один брокер не знает о данном типе сообщения")
        .Which.Message.Should().Contain(typeof(object).FullName);
    }

    [Fact]
    public void when_configure_without_egress_it_should_configure_router_expected_way_using_services_from_container()
    {
      var (routerConfiguration, router) = CreateRouterWithoutEgress();

      router.Should().NotBeNull();
      var brokerConfiguration = routerConfiguration.Brokers.Single();
      brokerConfiguration.HasNoEgress.Should().BeTrue("egress не сконфигурирован и должен быть недоступен");

      Action egressGetter = () =>
      {
        var _ = brokerConfiguration.Egress;
      };

      egressGetter.Should().ThrowExactly<InvalidOperationException>("egress не сконфигурирован и должен быть недоступен");
    }

    [Fact]
    public void when_configure_without_any_egress_and_publish_message_it_should_fail()
    {
      var (_, router) = CreateRouterWithoutEgress();
      Func<Task> sut = () => router.RouteEgressMessage(new object());
      sut.Should().ThrowExactly<PoezdOperationException>("egress не сконфигурирован")
        .Which.Message.Should().Contain(typeof(object).FullName);
    }

    private static (MessageRouterConfiguration, IMessageRouter) CreateRouterWithBothIngressAndEgress()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      var routerConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("venture-kafka")
                .Ingress(
                  ingress => ingress
                    .WithTestDriver()
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
                .Egress(
                  egress => egress
                    .WithTestDriver()
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

      container.RegisterSingleton(() => routerConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<EmptyHandlerRegistry>();
      container.RegisterSingleton<EmptyIngressApiMessageTypesRegistry>();
      container.RegisterSingleton<ProvidingNothingQueueNamePatternsProvider>();
      container.RegisterSingleton<EmptyEgressApiMessageTypesRegistry>();

      container.Verify();

      return (routerConfiguration, container.GetInstance<IMessageRouter>());
    }

    private static (MessageRouterConfiguration, IMessageRouter) CreateRouterWithoutEgress()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      var routerConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("venture-kafka")
                .Ingress(
                  ingress => ingress
                    .WithTestDriver()
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

      container.RegisterSingleton(() => routerConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<EmptyHandlerRegistry>();
      container.RegisterSingleton<EmptyIngressApiMessageTypesRegistry>();
      container.RegisterSingleton<ProvidingNothingQueueNamePatternsProvider>();

      container.Verify();

      return (routerConfiguration, container.GetInstance<IMessageRouter>());
    }
  }
}
