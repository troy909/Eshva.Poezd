#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.KafkaCoupling;
using Eshva.Poezd.SimpleInjectorCoupling;
using FluentAssertions;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.TestingTools.Kafka;
using Venture.IntegrationTests.TestSubjects;
using Xunit;
using Xunit.Abstractions;

#endregion


namespace Venture.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public sealed class given_kafka_server
  {
    public given_kafka_server(KafkaSetupContainerAsyncFixture fixture, [NotNull] ITestOutputHelper testOutputHelper)
    {
      if (fixture == null) throw new ArgumentNullException(nameof(fixture));
      _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));

      _kafkaTestContextFactory = new KafkaTestContextFactory(fixture.KafkaContainerConfiguration.BootstrapServers);
    }

    [Fact]
    public async Task when_message_published_to_kafka_topic_it_should_be_received_from_same_topic()
    {
      const string someTopic = "some-topic";
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string>(timeout);
      await kafkaTestContext.CreateTopics(someTopic);

      const string expectedValue = "Eshva";
      await kafkaTestContext.Produce(expectedValue, someTopic);
      var consumed = kafkaTestContext.Consume(someTopic);
      consumed.Should().Be(expectedValue);
    }

    [Fact]
    public async Task when_message_published_to_kafka_topic_it_should_be_received_by_properly_configured_poezd()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      const string someTopic = "some-topic";
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string>(timeout);
      await kafkaTestContext.CreateTopics(someTopic);

      const string expectedValue = "Eshva";
      await kafkaTestContext.Produce(expectedValue, someTopic);

      container.Register<MessageCountingPipelineConfigurator>(Lifestyle.Scoped);
      container.Register<CounterStep>(Lifestyle.Scoped);
      var counter = new CounterStep.Properties();
      container.RegisterInstance(counter);

      var messageRouter = GetMessageRouter<MessageCountingPipelineConfigurator>(container);
      await messageRouter.Start(timeout);

      counter.Counter.Should().Be(expected: 1, "one message has been sent");
      // var consumed = kafkaTestContext.Consume(someTopic);
      // consumed.Should().Be(expectedValue);
      // await messageRouter.RouteIncomingMessage(
      //   "sample-broker-server",
      //   "sample.facts.service-2.v1",
      //   DateTimeOffset.UtcNow,
      //   new byte[0],
      //   new Dictionary<string, string>());
    }

    private IMessageRouter GetMessageRouter<TBrokerPipelineConfigurator>(Container container)
      where TBrokerPipelineConfigurator : IPipelineConfigurator
    {
      ConfigurePoezd<TBrokerPipelineConfigurator>(container);
      var messageRouter = container.GetInstance<IMessageRouter>();
      return messageRouter;
    }

    private void ConfigurePoezd<TBrokerPipelineConfigurator>(Container container) where TBrokerPipelineConfigurator : IPipelineConfigurator
    {
      container.RegisterInstance<IServiceProvider>(container); // TODO: Do I need it?

      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("sample-broker-server")
                .WithDriver<KafkaDriverFactory, KafkaDriverConfigurator, KafkaDriverConfiguration>(
                  driver => driver
                    .WithConsumerConfig(CreateConsumerConfig())
                    .WithProducerConfig(CreateProducerConfig()))
                .WithQueueNameMatcher<RegexQueueNameMatcher>()
                .WithPipelineConfigurator<TBrokerPipelineConfigurator>()
                .AddPublicApi(
                  api => api
                    .WithId("api-1")
                    .AddQueueNamePattern(@"^some-.*")
                    .WithPipelineConfigurator<EmptyPipelineConfigurator>())));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      container.RegisterInstance(GetLoggerFactory());
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);

      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<EmptyPipelineConfigurator>();
      container.RegisterSingleton<KafkaDriverFactory>();
      // container.Register<SampleBrokerPipelineConfigurator>(Lifestyle.Scoped);
      // container.Register<Service1PipelineConfigurator>(Lifestyle.Scoped);
      // container.Register<Service2PipelineConfigurator>(Lifestyle.Scoped);
      // container.Register<CdcNotificationsPipelineConfigurator>(Lifestyle.Scoped);
      // container.Register<LogMessageHandlingContextStep>(Lifestyle.Scoped);
      // container.Register<CdcNotificationsCommitStep>(Lifestyle.Scoped);
      // container.Register<Service1DeserializeMessageStep>(Lifestyle.Scoped);
      // container.Register<Service2DeserializeMessageStep>(Lifestyle.Scoped);
      // container.Register<GetMessageHandlersStep>(Lifestyle.Scoped);
      // container.Register<DispatchMessageToHandlersStep>(Lifestyle.Scoped);
      // container.Register<CommitMessageStep>(Lifestyle.Scoped);

      container.Verify();
    }

    private static ProducerConfig CreateProducerConfig()
    {
      var producerConfig = new ProducerConfig
      {
        BootstrapServers = "localhost:9092",
        ApiVersionRequest = true,
        QueueBufferingMaxKbytes = 10240,
        Debug = "msg",
        MessageTimeoutMs = 3000
      };
      producerConfig.Set("request.required.acks", "-1");
      producerConfig.Set("queue.buffering.max.ms", "5");
      return producerConfig;
    }

    private static ConsumerConfig CreateConsumerConfig() =>
      new ConsumerConfig
      {
        BootstrapServers = "localhost:9092",
        ApiVersionRequest = true,
        GroupId = Guid.NewGuid().ToString("N"),
        EnableAutoCommit = false,
        FetchWaitMaxMs = 5,
        FetchErrorBackoffMs = 5,
        QueuedMinMessages = 1000,
        SessionTimeoutMs = 6000,
        StatisticsIntervalMs = 5000,
        TopicMetadataRefreshIntervalMs = 20000, // Otherwise it runs maybe five minutes
        Debug = "msg",
        AutoOffsetReset = AutoOffsetReset.Earliest,
        EnablePartitionEof = true
      };

    private ILoggerFactory GetLoggerFactory() =>
      new LoggerFactory().AddSerilog(
        new LoggerConfiguration()
          .WriteTo.InMemory()
          .WriteTo.XunitTestOutput(_testOutputHelper)
          .MinimumLevel.Verbose()
          .CreateLogger());

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
    private readonly ITestOutputHelper _testOutputHelper;
  }
}
