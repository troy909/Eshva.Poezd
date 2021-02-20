#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using FluentAssertions;
using SimpleInjector;
using Venture.CaseOffice.Messages;
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
    public given_kafka_server(KafkaSetupContainerAsyncFixture fixture, ITestOutputHelper testOutput)
    {
      if (fixture == null) throw new ArgumentNullException(nameof(fixture));
      _testOutput = testOutput;

      _kafkaTestContextFactory = new KafkaTestContextFactory(fixture.KafkaContainerConfiguration.BootstrapServers);
    }

    [Fact]
    public async Task when_message_published_to_kafka_topic_it_should_be_received_from_same_topic()
    {
      var topic = RoutingTests.GetRandomTopic();
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string>(timeout);
      await kafkaTestContext.CreateTopics(topic);

      var expectedValue = RoutingTests.GetRandomString();
      await kafkaTestContext.Produce(
        topic,
        expectedValue,
        new Dictionary<string, byte[]> {{"header1", new byte[0]}});
      var consumeResult = kafkaTestContext.Consume(topic);
      consumeResult.Message.Value.Should().Be(expectedValue, "this thing was sent");
      consumeResult.Message.Headers.Count.Should().Be(expected: 1, "one header was set");
    }

    [Fact]
    public async Task when_message_published_to_kafka_topic_it_should_be_received_by_properly_configured_poezd()
    {
      var container = RoutingTests.SetupContainer<MessageCountingPipeFitter, FinishTestPipeFitter>(
        api => api.WithId("case-office")
          .WithQueueNamePatternsProvider<PublicApi1QueueNamePatternsProvider>()
          .WithIngressPipeFitter<EmptyPipeFitter>()
          .WithMessageTypesRegistry<CaseOfficeMessageTypesRegistry>()
          .WithHandlerRegistry<EmptyHandlerRegistry>(),
        _testOutput);

      var topic = RoutingTests.GetRandomTopic();
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<byte[]>(timeout);
      await kafkaTestContext.CreateTopics(topic);

      var expectedValue = new byte[10];
      new Random().NextBytes(expectedValue);
      await kafkaTestContext.Produce(topic, expectedValue);
      // var expectedValue = RoutingTests.GetRandomString();
      // await kafkaTestContext.Produce(topic, expectedValue);

      container.RegisterSingleton<MessageCountingPipeFitter>();
      container.RegisterSingleton<CaseOfficeMessageTypesRegistry>();
      container.Register<CounterStep>(Lifestyle.Scoped);
      var counter = new CounterStep.Properties();
      container.RegisterInstance(counter);

      var testIsFinished = RoutingTests.AddTestFinishSemaphore(container);

      var router = container.GetMessageRouter();
      await router.Start(timeout);

      await testIsFinished.WaitAsync(timeout);
      counter.Counter.Should().Be(expected: 1, "one message has been sent");
    }

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
    private readonly ITestOutputHelper _testOutput;
  }
}
