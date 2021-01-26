#region Usings

using System;
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
      const string someTopic = "some-topic";
      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string>(timeout);
      await kafkaTestContext.CreateTopics(someTopic);

      const string expectedValue = "Eshva";
      await kafkaTestContext.Produce(expectedValue, someTopic);
      var consumed = kafkaTestContext.Consume(someTopic);
      consumed.Should().Be(expectedValue);
    }

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
  }
}
