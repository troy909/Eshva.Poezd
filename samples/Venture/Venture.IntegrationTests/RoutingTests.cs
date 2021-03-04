#region Usings

using System;
using System.Threading;
using Confluent.Kafka;
using Eshva.Common.Testing;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.KafkaCoupling;
using Eshva.Poezd.SimpleInjectorCoupling;
using RandomStringCreator;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.CaseOffice.Messages;
using Venture.Common.Poezd.Adapter.MessageHandling;
using Venture.IntegrationTests.TestSubjects;
using Xunit.Abstractions;

#endregion

namespace Venture.IntegrationTests
{
  public static class RoutingTests
  {
    public static Container SetupContainer<TIngressEnterPipeline, TIngressExitPipeline, TEgressEnterPipeline, TEgressExitPipeline>(
      Action<PublicApiConfigurator> configureApi,
      ITestOutputHelper testOutput)
      where TIngressEnterPipeline : IPipeFitter
      where TIngressExitPipeline : IPipeFitter
      where TEgressEnterPipeline : IPipeFitter
      where TEgressExitPipeline : IPipeFitter
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(testOutput)
        .AddRouter<TIngressEnterPipeline, TIngressExitPipeline, TEgressEnterPipeline, TEgressExitPipeline>(configureApi);

      container.RegisterInstance<IServiceProvider>(container);
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<KafkaDriverFactory>();
      container.RegisterSingleton<Utf8ByteStringHeaderValueParser>();
      container.RegisterSingleton<PublicApi1QueueNamePatternsProvider>();
      container.RegisterSingleton<EmptyHandlerRegistry>();

      return container;
    }

    public static IMessageRouter GetMessageRouter(this Container container) => container.GetInstance<IMessageRouter>();

    public static string GetRandomString() => StringCreator.Get(length: 10);

    public static string GetRandomTopic(string prefix = TopicPrefix) => $"{prefix}-{GetRandomString()}";

    public static SemaphoreSlim AddTestFinishSemaphore(Container container)
    {
      container.RegisterSingleton<FinishTestPipeFitter>();
      container.Register<FinishTestStep>(Lifestyle.Scoped);
      var testIsFinished = new FinishTestStep.Properties();
      container.RegisterInstance(testIsFinished);
      return testIsFinished.Semaphore;
    }

    private static Container AddRouter<TIngressEnterPipeline, TIngressExitPipeline, TEgressEnterPipeline, TEgressExitPipeline>(
      this Container container,
      Action<PublicApiConfigurator> configureApi)
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
                .WithId("venture-kafka")
                .WithDriver<KafkaDriverFactory, KafkaDriverConfigurator, KafkaDriverConfiguration>(
                  driver => driver
                    .WithConsumerConfig(CreateConsumerConfig())
                    .WithProducerConfig(CreateProducerConfig())
                    .WithCommitPeriod(commitPeriod: 1)
                    .WithHeaderValueParser<Utf8ByteStringHeaderValueParser>())
                .WithQueueNameMatcher<RegexQueueNameMatcher>()
                .WithIngressEnterPipeFitter<TIngressEnterPipeline>()
                .WithIngressExitPipeFitter<TIngressExitPipeline>()
                .WithEgressEnterPipeFitter<TEgressEnterPipeline>()
                .WithEgressExitPipeFitter<TEgressExitPipeline>()
                .AddPublicApi(configureApi)));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container;
    }

    private static Container AddRouter1<TIngressEnterPipeline, TIngressExitPipeline, TEgressEnterPipeline, TEgressExitPipeline>(
      this Container container,
      Action<PublicApiConfigurator> configureApi)
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
                .WithId("venture-kafka")
                .Ingress(
                  ingress => ingress
                    .WithKafkaDriver(
                      driver => driver
                        .WithConsumerConfig(CreateConsumerConfig())
                        .WithHeaderValueParser<Utf8ByteStringHeaderValueParser>())
                    .WithEnterPipeFitter<TIngressEnterPipeline>()
                    .WithExitPipeFitter<TIngressExitPipeline>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddPublicApi(
                      api => api
                        .WithId("case-office-ingress")
                        .WithQueueNamePatternsProvider<VentureQueueNamePatternsProvider>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageTypesRegistry<CaseOfficeIngressMessageTypesRegistry>()
                        .WithHandlerRegistry<VentureServiceHandlersRegistry>()))
                .Egress(
                  egress => egress
                    .WithKafkaDriver(
                      driver => driver
                        .WithProducerConfig(CreateProducerConfig()))
                    .WithEnterPipeFitter<TEgressEnterPipeline>()
                    .WithExitPipeFitter<TEgressExitPipeline>()
                    .AddPublicApi(
                      api => api
                        .WithId("case-office-egress")
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageTypesRegistry<CaseOfficeEgressMessageTypesRegistry>()))));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      return container;
    }

    private static ConsumerConfig CreateConsumerConfig() =>
      new ConsumerConfig
      {
        BootstrapServers = "localhost:9092",
        ApiVersionRequest = true,
        ClientId = "case-office-service-client",
        GroupId = Guid.NewGuid().ToString("N"),
        EnableAutoCommit = true,
        FetchWaitMaxMs = 5,
        FetchErrorBackoffMs = 5,
        QueuedMinMessages = 1000,
        SessionTimeoutMs = 6000,
        StatisticsIntervalMs = 5000,
        TopicMetadataRefreshIntervalMs = 20000, // Otherwise it runs maybe five minutes
        //Debug = "msg",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnablePartitionEof = true,
        AllowAutoCreateTopics = true
      };

    private static ProducerConfig CreateProducerConfig()
    {
      var producerConfig = new ProducerConfig
      {
        BootstrapServers = "localhost:9092",
        ApiVersionRequest = true,
        QueueBufferingMaxKbytes = 10240,
        //Debug = "msg",
        MessageTimeoutMs = 3000
      };
      producerConfig.Set("request.required.acks", "-1");
      producerConfig.Set("queue.buffering.max.ms", "5");
      return producerConfig;
    }

    private const string TopicPrefix = @"some";
    private static readonly StringCreator StringCreator = new StringCreator();
  }
}
