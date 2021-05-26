#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Common.Testing;
using Eshva.Poezd.Adapter.Kafka;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Adapter.SimpleInjector;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using JetBrains.Annotations;
using RandomStringCreator;
using SimpleInjector;
using SimpleInjector.Lifestyles;
using Venture.CaseOffice.Messages;
using Venture.Common.Poezd.Adapter.Ingress;
using Venture.Common.TestingTools.Core;
using Venture.Common.TestingTools.Kafka;
using Venture.IntegrationTests.TestSubjects;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Venture.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public sealed class given_kafka_server_and_message_router
  {
    public given_kafka_server_and_message_router(KafkaSetupContainerAsyncFixture fixture, ITestOutputHelper testOutput)
    {
      if (fixture == null) throw new ArgumentNullException(nameof(fixture));
      _testOutput = testOutput;
      _kafkaTestContextFactory = new KafkaTestContextFactory(fixture.KafkaContainerConfiguration.BootstrapServers);
    }

    [Fact]
    public async Task when_message_published_to_kafka_topic_it_should_be_received_by_properly_configured_poezd()
    {
      var timeoutOrDoneSource = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5));
      var doneOrTimeout = timeoutOrDoneSource.Token;
      var topic = RoutingTests.GetRandomTopic();
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string, string>(doneOrTimeout);
      await kafkaTestContext.CreateTopics(topic);

      var enterProperties = new MessageLoggingStep.Properties();
      var exitProperties = new MessageCountingStep.Properties(expectedMessageCount: 2, timeoutOrDoneSource);

      var publishedKey1 = CreateString(length: 10);
      var publishedPayload1 = CreateString(length: 10);

      await ProduceMessage(
        topic,
        publishedKey1,
        publishedPayload1,
        kafkaTestContext);

      var container = BuildContainerToHandleMessages(enterProperties, exitProperties);
      container.RegisterSingleton(
        () => CreateMessageRouterConfigurationToHandleMessages().CreateMessageRouter(new SimpleInjectorAdapter(container)));
      await container.GetInstance<IMessageRouter>().Start(doneOrTimeout);

      var publishedKey2 = CreateString(length: 20);
      var publishedPayload2 = CreateString(length: 20);
      await ProduceMessage(
        topic,
        publishedKey2,
        publishedPayload2,
        kafkaTestContext);

      await doneOrTimeout;

      enterProperties.Messages.Select(message => (string) message.Key).Should().Equal(
        new[] {publishedKey1, publishedKey2},
        "these messages were published in this order");
      enterProperties.Messages.Select(message => (string) message.Value).Should().Equal(
        new[] {publishedPayload1, publishedPayload2},
        "these messages were published in this order");
    }

    private string CreateString(int length) => _stringCreator.Get(length);

    private Task ProduceMessage(
      string topic,
      string publishedKey,
      string publishedPayload,
      KafkaTestContext<string, string> kafkaTestContext)
    {
      _testOutput.WriteLine($"Published: {publishedPayload}");
      return kafkaTestContext.Produce(
        topic,
        publishedKey,
        publishedPayload);
    }

    private static MessageRouterConfiguration CreateMessageRouterConfigurationToHandleMessages()
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
                        .WithConsumerFactory<DefaultApiConsumerFactory>()
                        .WithDeserializerFactory<DefaultDeserializerFactory>()
                        .WithConsumerConfigurator<VentureConsumerConfigurator>()
                        .WithHeaderValueCodec<Utf8ByteStringHeaderValueCodec>())
                    .WithEnterPipeFitter<EnterPipeFitter>()
                    .WithExitPipeFitter<ExitPipeFitter>()
                    .WithQueueNameMatcher<RegexQueueNameMatcher>()
                    .AddApi(
                      api => api
                        .WithId("ingress-case-office")
                        .WithQueueNamePatternsProvider<IngressApi1QueueNamePatternsProvider>()
                        .WithMessageKey<string>()
                        .WithMessagePayload<string>()
                        .WithPipeFitter<EmptyPipeFitter>()
                        .WithMessageTypesRegistry<CaseOfficeIngressApiMessageTypesRegistry>()
                        .WithHandlerRegistry<EmptyHandlerRegistry>()))
                .WithoutEgress()));
      return messageRouterConfiguration;

      static ConsumerConfig CreateConsumerConfig()
      {
        return new ConsumerConfig
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
          // Debug = "msg",
          AutoOffsetReset = AutoOffsetReset.Earliest,
          EnablePartitionEof = true,
          AllowAutoCreateTopics = false
        };
      }
    }

    private Container BuildContainerToHandleMessages(
      MessageLoggingStep.Properties enterProperties,
      MessageCountingStep.Properties exitProperties)
    {
      var container = new Container();
      container.Options.DefaultScopedLifestyle = new AsyncScopedLifestyle();
      container.AddLogging(_testOutput);
      container.RegisterInstance<IDiContainerAdapter>(new SimpleInjectorAdapter(container));
      container.RegisterSingleton<VentureConsumerConfigurator>();
      container.RegisterSingleton<IConsumerFactory, DefaultConsumerFactory>();
      container.RegisterSingleton<IDeserializerFactory, DefaultDeserializerFactory>();
      container.RegisterSingleton<IApiConsumerFactory, DefaultApiConsumerFactory>();
      container.RegisterSingleton<DefaultApiConsumerFactory>();
      container.RegisterSingleton<IConsumerConfigurator, VentureConsumerConfigurator>();
      container.RegisterSingleton<RegexQueueNameMatcher>();
      container.RegisterSingleton<EmptyPipeFitter>();
      container.RegisterSingleton<IHeaderValueCodec, Utf8ByteStringHeaderValueCodec>();
      container.RegisterSingleton<Utf8ByteStringHeaderValueCodec>();
      container.RegisterSingleton<IngressApi1QueueNamePatternsProvider>();
      container.RegisterSingleton<EmptyHandlerRegistry>();
      container.RegisterSingleton<CaseOfficeIngressApiMessageTypesRegistry>();
      container.RegisterSingleton<EnterPipeFitter>();
      container.Register<MessageLoggingStep>(Lifestyle.Scoped);

      container.RegisterSingleton<ExitPipeFitter>();
      container.Register<MessageCountingStep>(Lifestyle.Scoped);
      container.RegisterInstance(exitProperties);
      container.RegisterInstance(enterProperties);

      return container;
    }

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
    private readonly StringCreator _stringCreator = new StringCreator();
    private readonly ITestOutputHelper _testOutput;

    private class IngressApi1QueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      public IEnumerable<string> GetQueueNamePatterns()
      {
        yield return "^some-";
      }
    }

    private class EnterPipeFitter : TypeBasedPipeFitter
    {
      public EnterPipeFitter([NotNull] IDiContainerAdapter serviceProvider) : base(serviceProvider) { }

      protected override IEnumerable<Type> GetStepTypes()
      {
        yield return typeof(MessageLoggingStep);
      }
    }

    private class MessageLoggingStep : IStep<MessageHandlingContext>
    {
      public MessageLoggingStep(Properties props)
      {
        _props = props;
      }

      public Task Execute(MessageHandlingContext context)
      {
        _props.Messages.Add(new KeyValuePair<object, object>(context.Key, context.Payload));
        return Task.CompletedTask;
      }

      private readonly Properties _props;

      public class Properties
      {
        public List<KeyValuePair<object, object>> Messages { get; } = new List<KeyValuePair<object, object>>();
      }
    }

    private class ExitPipeFitter : TypeBasedPipeFitter
    {
      public ExitPipeFitter([NotNull] IDiContainerAdapter serviceProvider) : base(serviceProvider) { }

      protected override IEnumerable<Type> GetStepTypes()
      {
        yield return typeof(MessageCountingStep);
      }
    }

    private class MessageCountingStep : IStep<MessageHandlingContext>
    {
      public MessageCountingStep(Properties properties)
      {
        _properties = properties;
      }

      public Task Execute(MessageHandlingContext context)
      {
        _properties.MessageCount++;
        if (_properties.MessageCount == _properties.ExpectedMessageCount) _properties.TokenSource.Cancel();
        return Task.CompletedTask;
      }

      private readonly Properties _properties;

      public class Properties
      {
        public Properties(uint expectedMessageCount, CancellationTokenSource tokenSource)
        {
          ExpectedMessageCount = expectedMessageCount;
          TokenSource = tokenSource;
        }

        public uint ExpectedMessageCount { get; }

        public CancellationTokenSource TokenSource { get; }

        public uint MessageCount { get; set; }
      }
    }
  }
}
