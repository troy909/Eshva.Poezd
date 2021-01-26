#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.KafkaCoupling;
using Eshva.Poezd.SimpleInjectorCoupling;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.Common.TestingTools.Kafka;
using Xunit;
using Xunit.Abstractions;

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
      const string someTopic = "some-topic";
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string>(timeout);
      await kafkaTestContext.CreateTopics(someTopic);

      const string expectedValue = "Eshva";
      await kafkaTestContext.Produce(expectedValue, someTopic);

      // var consumed = kafkaTestContext.Consume(someTopic);
      // consumed.Should().Be(expectedValue);
    }

    private void ConfigurePoezd(Container container)
    {
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.RegisterInstance<IServiceProvider>(container);
      var messageRouterConfiguration =
        MessageRouter.Configure(
          router => router
            .AddMessageBroker(
              broker => broker.WithId("sample-broker-server")
                .WithDriver<KafkaDriver, KafkaDriverConfigurator, KafkaDriverConfiguration>(
                  driver => driver.WithSomeSetting("some setting"))
                .WithQueueNameMatcher<RegexQueueNameMatcher>()
                .WithPipelineConfigurator<EmptyPipelineConfigurator>()
                .AddPublicApi(
                  api => api.WithId("api-1")
                    .AddQueueNamePattern(@"^sample\.(commands|facts)\.service1\.v1")
                    .WithPipelineConfigurator<EmptyPipelineConfigurator>())));

      container.RegisterSingleton(() => messageRouterConfiguration.CreateMessageRouter(new SimpleInjectorAdapter(container)));
      container.RegisterInstance(GetLoggerFactory());
      container.Register(
        typeof(ILogger<>),
        typeof(Logger<>),
        Lifestyle.Singleton);

      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<EmptyPipelineConfigurator>();
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
