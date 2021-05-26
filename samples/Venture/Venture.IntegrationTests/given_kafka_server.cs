#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using Venture.Common.TestingTools.Kafka;
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
      var topic = RoutingTests.GetRandomTopic();
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string, string>(timeout);
      await kafkaTestContext.CreateTopics(topic);

      var expectedValue = RoutingTests.GetRandomString();
      await kafkaTestContext.Produce(
        topic,
        string.Empty,
        expectedValue,
        new Dictionary<string, byte[]> {{"header1", new byte[0]}});
      var consumeResult = kafkaTestContext.Consume(topic);
      consumeResult.Message.Value.Should().Be(expectedValue, "this thing was sent");
      consumeResult.Message.Headers.Count.Should().Be(expected: 1, "one header was set");
    }

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
  }
}
