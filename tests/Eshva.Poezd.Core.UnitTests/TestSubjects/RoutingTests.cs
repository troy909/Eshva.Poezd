#region Usings

using Eshva.Common.Testing;
using Eshva.Poezd.Adapter.SimpleInjector;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using RandomStringCreator;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public static class RoutingTests
  {
    public static Container SetupContainer(ITestOutputHelper testOutput)
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(testOutput);

      container.RegisterInstance<IDiContainerAdapter>(new SimpleInjectorAdapter(container));
      container.RegisterSingleton<ThrowingEgressStep>();
      container.RegisterSingleton<WithThrowingStepPipeFitter>();
      container.RegisterSingleton<TestBrokerEgressEnterStep>();
      container.RegisterSingleton<TestBrokerEgressExitStep>();
      container.RegisterSingleton<TestBrokerEgressApiStep>();
      container.RegisterSingleton<TestBrokerEgressExitPipeFitter>();
      container.RegisterSingleton<TestBrokerEgressEnterPipeFitter>();
      container.RegisterSingleton<TestBrokerEgressApiPipeFitter>();
      container.RegisterSingleton<OwningEverythingEgressApiMessageTypesRegistry>();
      container.RegisterSingleton<WithBreakingHandlerPipeFitter>();
      container.RegisterSingleton<BreakingIngressStep>();
      container.RegisterSingleton<WithThrowingHandlerPipeFitter>();
      container.RegisterSingleton<ThrowingIngressStep>();
      container.RegisterSingleton<MatchingNothingQueueNameMatcher>();
      container.RegisterSingleton<MatchingEverythingQueueNameMatcher>();
      container.RegisterSingleton<ProvidingNothingQueueNamePatternsProvider>();
      container.RegisterSingleton<EmptyIngressApiMessageTypesRegistry>();
      container.RegisterSingleton<EmptyEgressApiMessageTypesRegistry>();
      container.RegisterSingleton<EmptyHandlerRegistry>();
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<Service1QueueNamePatternsProvider>();
      container.RegisterSingleton<Service2QueueNamePatternsProvider>();
      container.RegisterSingleton<CdcNotificationsQueueNamePatternsProvider>();
      container.RegisterSingleton<SampleBrokerPipeFitter>();
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<Service1PipeFitter>();
      container.RegisterSingleton<Service2PipeFitter>();
      container.RegisterSingleton<CdcNotificationsPipeFitter>();
      container.Register<LogMessageHandlingContextStep>(Lifestyle.Scoped);
      container.Register<CdcNotificationsCommitStep>(Lifestyle.Scoped);
      container.Register<Service1DeserializeMessageStep>(Lifestyle.Scoped);
      container.Register<Service2DeserializeMessageStep>(Lifestyle.Scoped);
      container.Register<GetMessageHandlersStep>(Lifestyle.Scoped);
      container.Register<DispatchMessageToHandlersStep>(Lifestyle.Scoped);
      container.Register<CommitMessageStep>(Lifestyle.Scoped);

      return container;
    }

    public static IMessageRouter GetMessageRouter(this Container container) => container.GetInstance<IMessageRouter>();

    public static Container AddRouterWithComplexApis<TIngressEnterPipeline, TIngressExitPipeline, TEgressEnterPipeline,
      TEgressExitPipeline>(this Container container, TestDriverState state)
      where TIngressEnterPipeline : IPipeFitter
      where TIngressExitPipeline : IPipeFitter
      where TEgressEnterPipeline : IPipeFitter
      where TEgressExitPipeline : IPipeFitter
    {
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId(SampleBrokerServer)
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<TIngressEnterPipeline>()
                    .WithExitPipeFitter<TIngressExitPipeline>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("api-1-ingress")
                        .WithQueueNamePatternsProvider<Service1QueueNamePatternsProvider>()
                        .WithPipeFitter<Service1PipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>())
                    .AddApi(
                      api => api
                        .WithId("api-2-ingress")
                        .WithQueueNamePatternsProvider<Service2QueueNamePatternsProvider>()
                        .WithPipeFitter<Service2PipeFitter>()
                        .WithMessageKey<long>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>())
                    .AddApi(
                      api => api
                        .WithId("cdc-notifications-ingress")
                        .WithQueueNamePatternsProvider<CdcNotificationsQueueNamePatternsProvider>()
                        .WithPipeFitter<CdcNotificationsPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<string>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>()))
                .Egress(
                  egress => egress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<TEgressEnterPipeline>()
                    .WithExitPipeFitter<TEgressExitPipeline>()
                    .AddApi(
                      api => api
                        .WithId("case-office-egress")
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyEgressApiMessageTypesRegistry>()))));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container;
    }

    public static Container AddRouterWithThrowingHandler(
      this Container container,
      TestDriverState state,
      string brokerName)
    {
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId(brokerName)
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<WithThrowingHandlerPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .WithQueueNameMatcher<MatchingEverythingQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("api-1-ingress")
                        .WithQueueNamePatternsProvider<Service1QueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>()))
                .Egress(
                  egress => egress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<WithThrowingStepPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .AddApi(
                      api => api
                        .WithId("api-1-egress")
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<OwningEverythingEgressApiMessageTypesRegistry>()))));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container;
    }

    public static Container AddRouterWithBreakHandlingStep(
      this Container container,
      TestDriverState state,
      string brokerName)
    {
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId(brokerName)
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<WithBreakingHandlerPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .WithQueueNameMatcher<MatchingEverythingQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("api-1-ingress")
                        .WithQueueNamePatternsProvider<Service1QueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>()))
                .Egress(
                  egress => egress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .AddApi(
                      api => api
                        .WithId("api-1-egress")
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyEgressApiMessageTypesRegistry>()))));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container;
    }

    public static Container AddRouterWithThreeBrokers(
      this Container container,
      string broker1Name,
      string broker2Name,
      string broker3Name,
      TestDriverState state)
    {
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId(broker1Name)
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .WithQueueNameMatcher<MatchingNothingQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("api-1-ingress")
                        .WithQueueNamePatternsProvider<ProvidingNothingQueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>()))
                .Egress(
                  egress => egress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .AddApi(
                      api => api
                        .WithId("api-1-egress")
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyEgressApiMessageTypesRegistry>())))
            .AddMessageBroker(
              broker => broker
                .WithId(broker2Name)
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .WithQueueNameMatcher<MatchingNothingQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("api-2-ingress")
                        .WithQueueNamePatternsProvider<ProvidingNothingQueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>()))
                .Egress(
                  egress => egress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .AddApi(
                      api => api
                        .WithId("api-2-egress")
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyEgressApiMessageTypesRegistry>())))
            .AddMessageBroker(
              broker => broker
                .WithId(broker3Name)
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .WithQueueNameMatcher<MatchingNothingQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("api-3-ingress")
                        .WithQueueNamePatternsProvider<ProvidingNothingQueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>()))
                .Egress(
                  egress => egress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .AddApi(
                      api => api
                        .WithId("api-3-egress")
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyEgressApiMessageTypesRegistry>()))));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container;
    }

    public static Container AddRouterWithConfiguredEgressApi(this Container container, TestDriverState state)
    {
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("broker-1")
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>()
                    .WithQueueNameMatcher<MatchingNothingQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("api-1-ingress")
                        .WithQueueNamePatternsProvider<ProvidingNothingQueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<EmptyIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>()))
                .Egress(
                  egress => egress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<TestBrokerEgressEnterPipeFitter>()
                    .WithExitPipeFitter<TestBrokerEgressExitPipeFitter>()
                    .AddApi(
                      api => api
                        .WithId("api-1-egress")
                        .WithPipeFitter<TestBrokerEgressApiPipeFitter>()
                        .WithMessageKey<int>()
                        .WithMessagePayload<byte[]>()
                        .WithMessageTypesRegistry<OwningEverythingEgressApiMessageTypesRegistry>()))));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container;
    }

    public static Container AddRouterWithInvalidConfiguration(this Container container, TestDriverState state)
    {
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("invalid configured broker")
                .Ingress(
                  ingress => ingress
                    .WithTestDriver(state)
                    .WithEnterPipeFitter<EmptyPipeFitter>()
                    .WithExitPipeFitter<EmptyPipeFitter>())));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container;
    }

    public static string RandomString(uint length = 10) => StringCreator.Get((int) length);

    public const string SampleBrokerServer = "sample-broker-server";
    private static readonly StringCreator StringCreator = new StringCreator();
  }
}
