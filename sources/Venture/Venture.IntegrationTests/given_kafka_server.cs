#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.KafkaCoupling;
using Eshva.Poezd.SimpleInjectorCoupling;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using RandomStringCreator;
using Serilog;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.TestingTools.Kafka;
using Venture.IntegrationTests.TestSubjects;
using Xunit;

#endregion


namespace Venture.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public sealed class given_kafka_server
  {
    public given_kafka_server(KafkaSetupContainerAsyncFixture fixture)
    {
      if (fixture == null) throw new ArgumentNullException(nameof(fixture));

      _kafkaTestContextFactory = new KafkaTestContextFactory(fixture.KafkaContainerConfiguration.BootstrapServers);
    }

    [Fact]
    public async Task when_message_published_to_kafka_topic_it_should_be_received_from_same_topic()
    {
      var topic = GetRandomTopic();
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string>(timeout);
      await kafkaTestContext.CreateTopics(topic);

      var expectedValue = GetRandomString();
      await kafkaTestContext.Produce(
        topic,
        expectedValue,
        new Dictionary<string, byte[]> {{"header1", new byte[0]}});
      var consumeResult = kafkaTestContext.Consume(topic);
      consumeResult.Message.Value.Should().Be(expectedValue, "this value was sent");
      consumeResult.Message.Headers.Count.Should().Be(expected: 1, "one header was set");
    }

    [Fact]
    public async Task when_message_published_to_kafka_topic_it_should_be_received_by_properly_configured_poezd()
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();

      var topic = GetRandomTopic();
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string>(timeout);
      await kafkaTestContext.CreateTopics(topic);

      var expectedValue = GetRandomString();
      await kafkaTestContext.Produce(topic, expectedValue);

      container.RegisterSingleton<MessageCountingPipelineConfigurator>();
      container.Register<CounterStep>(Lifestyle.Scoped);
      var counter = new CounterStep.Properties();
      container.RegisterInstance(counter);

      container.RegisterSingleton<FinishTestPipelineConfigurator>();
      container.Register<FinishTestStep>(Lifestyle.Scoped);
      var testIsFinished = new FinishTestStep.Properties();
      container.RegisterInstance(testIsFinished);

      var messageRouter = GetMessageRouter<MessageCountingPipelineConfigurator, FinishTestPipelineConfigurator>(container);
      await messageRouter.Start(timeout);

      await testIsFinished.Semaphore.WaitAsync(timeout);
      counter.Counter.Should().Be(expected: 1, "one message has been sent");
    }

    private IMessageRouter GetMessageRouter<TIngressEnterPipeline, TIngressExitPipeline>(Container container)
      where TIngressEnterPipeline : IPipelineConfigurator
      where TIngressExitPipeline : IPipelineConfigurator
    {
      ConfigurePoezd<TIngressEnterPipeline, TIngressExitPipeline>(container);
      var messageRouter = container.GetInstance<IMessageRouter>();
      return messageRouter;
    }

    private void ConfigurePoezd<TIngressEnterPipeline, TIngressExitPipeline>(Container container)
      where TIngressEnterPipeline : IPipelineConfigurator
      where TIngressExitPipeline : IPipelineConfigurator
    {
      container.RegisterInstance<IServiceProvider>(container);

      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker
                .WithId("sample-broker-server")
                .WithDriver<KafkaDriverFactory, KafkaDriverConfigurator, KafkaDriverConfiguration>(
                  driver => driver
                    .WithConsumerConfig(CreateConsumerConfig())
                    .WithProducerConfig(CreateProducerConfig())
                    .WithCommitPeriod(commitPeriod: 1)
                    .WithHeaderValueParser<Utf8ByteStringHeaderValueParser>())
                .WithQueueNameMatcher<RegexQueueNameMatcher>()
                .WithIngressEnterPipelineConfigurator<TIngressEnterPipeline>()
                .WithIngressExitPipelineConfigurator<TIngressExitPipeline>()
                .AddPublicApi(
                  api => api
                    .WithId("api-1")
                    .WithQueueNamePatternsProvider<PublicApi1QueueNamePatternsProvider>()
                    .WithIngressPipelineConfigurator<EmptyPipelineConfigurator>()
                    .WithHandlerFactory<PublicApi1HandlerFactory>())));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      container.RegisterInstance(GetLoggerFactory());
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);

      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<EmptyPipelineConfigurator>();
      container.RegisterSingleton<KafkaDriverFactory>();
      container.RegisterSingleton<Utf8ByteStringHeaderValueParser>();
      container.RegisterSingleton<PublicApi1QueueNamePatternsProvider>();

      container.Verify();
    }

    private static ConsumerConfig CreateConsumerConfig() =>
      new ConsumerConfig
      {
        BootstrapServers = "localhost:9092",
        ApiVersionRequest = true,
        ClientId = "test ClientId",
        GroupId = Guid.NewGuid().ToString("N"),
        EnableAutoCommit = false,
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

    private ILoggerFactory GetLoggerFactory() =>
      new LoggerFactory().AddSerilog(
        new LoggerConfiguration()
          .WriteTo.InMemory()
          .MinimumLevel.Verbose()
          .CreateLogger());

    private string GetRandomString() => _stringCreator.Get(length: 10);

    private string GetRandomTopic(string prefix = TopicPrefix) => $"{prefix}-{GetRandomString()}";

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
    private readonly StringCreator _stringCreator = new StringCreator();
    private const string TopicPrefix = @"some";
  }
}
