#region Usings

using System;
using System.Threading;
using Confluent.Kafka;
using Eshva.Common.Testing;
using Eshva.Poezd.Adapter.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Adapter.SimpleInjector;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using RandomStringCreator;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.Poezd.Adapter.Ingress;
using Venture.IntegrationTests.TestSubjects;
using Xunit.Abstractions;

#endregion

namespace Venture.IntegrationTests
{
  public static class RoutingTests
  {
    public static Container SetupContainer<TIngressEnterPipeline, TIngressExitPipeline, TEgressEnterPipeline, TEgressExitPipeline>(
      Action<IngressApiConfigurator> configureIngressApi,
      Action<EgressApiConfigurator> configureEgressApi,
      ITestOutputHelper testOutput)
      where TIngressEnterPipeline : IPipeFitter
      where TIngressExitPipeline : IPipeFitter
      where TEgressEnterPipeline : IPipeFitter
      where TEgressExitPipeline : IPipeFitter
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container
        .AddLogging(testOutput)
        .AddRouter<TIngressEnterPipeline, TIngressExitPipeline, TEgressEnterPipeline, TEgressExitPipeline>(
          configureIngressApi,
          configureEgressApi);

      container.RegisterInstance<IDiContainerAdapter>(new SimpleInjectorAdapter(container));
      container.RegisterInstance<IClock>(new TestClock(DateTimeOffset.UtcNow));
      container.RegisterSingleton<DefaultProducerFactory>();
      container.RegisterSingleton<VentureProducerConfigurator>();
      container.RegisterSingleton<DefaultSerializerFactory>();
      container.RegisterSingleton<VentureConsumerConfigurator>();
      container.RegisterSingleton<DefaultConsumerFactory>();
      container.RegisterSingleton<DefaultDeserializerFactory>();
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<Utf8ByteStringHeaderValueCodec>();
      container.RegisterSingleton<IngressApi1QueueNamePatternsProvider>();
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
      Action<IngressApiConfigurator> configureIngressApi,
      Action<EgressApiConfigurator> configureEgressApi)
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
                        .WithConsumerFactory<DefaultConsumerFactory>()
                        .WithDeserializerFactory<DefaultDeserializerFactory>()
                        .WithConsumerConfigurator<VentureConsumerConfigurator>()
                        .WithHeaderValueCodec<Utf8ByteStringHeaderValueCodec>())
                    .WithEnterPipeFitter<TIngressEnterPipeline>()
                    .WithExitPipeFitter<TIngressExitPipeline>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddApi(configureIngressApi))
                .Egress(
                  egress => egress
                    .WithKafkaDriver(
                      driver => driver
                        .WithProducerConfig(CreateProducerConfig())
                        .WithDefaultProducerFactory()
                        .WithSerializerFactory<DefaultSerializerFactory>()
                        .WithProducerConfigurator<VentureProducerConfigurator>()
                        .WithHeaderValueCodec<Utf8ByteStringHeaderValueCodec>())
                    .WithEnterPipeFitter<TEgressEnterPipeline>()
                    .WithExitPipeFitter<TEgressExitPipeline>()
                    .AddApi(configureEgressApi))));

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
      producerConfig.Set(@"request.required.acks", "-1");
      producerConfig.Set(@"queue.buffering.max.ms", "5");
      return producerConfig;
    }

    private const string TopicPrefix = @"some";
    private static readonly StringCreator StringCreator = new StringCreator();
  }
}
