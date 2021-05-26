#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
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
      router.Start(CancellationToken.None);

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
    public async Task when_configure_without_any_egress_and_publish_message_it_should_fail()
    {
      var (_, router) = CreateRouterWithoutEgress();
      await router.Start(CancellationToken.None);

      Func<Task> sut = () => router.RouteEgressMessage(new object());
      sut.Should().ThrowExactly<PoezdOperationException>("egress не сконфигурирован и должен быть недоступен")
        .Which.Message.Should().Contain("egress");
    }

    [Fact]
    public void when_configure_without_ingress_it_should_configure_router_expected_way_using_services_from_container()
    {
      var (routerConfiguration, router) = CreateRouterWithoutIngress();

      router.Should().NotBeNull();
      var brokerConfiguration = routerConfiguration.Brokers.Single();
      brokerConfiguration.HasNoIngress.Should().BeTrue("ingress не сконфигурирован и должен быть недоступен");

      Action egressGetter = () =>
      {
        var _ = brokerConfiguration.Ingress;
      };

      egressGetter.Should().ThrowExactly<InvalidOperationException>("ingress не сконфигурирован и должен быть недоступен");
    }

    [Fact]
    public async Task when_configure_without_any_ingress_and_driver_route_ingress_message_it_should_fail()
    {
      var (_, router) = CreateRouterWithoutIngress();
      await router.Start(CancellationToken.None);

      Func<Task> sut = () => router.RouteIngressMessage(
        BrokerId,
        "any queue",
        DateTimeOffset.Now,
        "any key",
        "any payload",
        new Dictionary<string, string>());

      sut.Should().ThrowExactly<PoezdOperationException>("ingress не сконфигурирован и должен быть недоступен")
        .Which.Message.Should().Contain("ingress");
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
                .WithId(BrokerId)
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

    private static (MessageRouterConfiguration, IMessageRouter) CreateRouterWithoutIngress()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      var routerConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId(BrokerId)
                .WithoutIngress()
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
                .WithId(BrokerId)
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

    private const string BrokerId = "single broker";
  }
}
