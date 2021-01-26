#region Usings

using System;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using FluentAssertions;
using Xunit;

#endregion


namespace Venture.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public sealed class given_kafka_server
  {
    public given_kafka_server(KafkaSetupContainerAsyncFixture fixture)
    {
      _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
    }

    [Fact]
    public async Task when_message_published_to_kafka_topic_it_should_be_received_by_poezd_with_correct_configuration()
    {
      var bootstrapServers = "localhost:9092";

      const string someTopic = "some-topic";
      const string expectedValue = "Eshva1";

      var topic = new TopicSpecification {Name = someTopic, NumPartitions = 1};
      var adminClientConfig = new AdminClientConfig {BootstrapServers = bootstrapServers};
      using (var adminClient = new AdminClientBuilder(adminClientConfig).Build())
      {
        try
        {
          await adminClient.CreateTopicsAsync(new[] {topic});
        }
        catch (Exception exception)
        {
          Console.WriteLine(exception);
          throw;
        }
      }


      var producerConfig = new ProducerConfig {BootstrapServers = bootstrapServers};
      using (var producer = new ProducerBuilder<Null, string>(producerConfig).Build())
      {
        try
        {
          await producer.ProduceAsync(someTopic, new Message<Null, string> {Value = expectedValue});
        }
        catch (Exception exception)
        {
          Console.WriteLine(exception);
          throw;
        }
      }

      var consumerConfig = new ConsumerConfig
      {
        BootstrapServers = bootstrapServers,
        GroupId = Guid.NewGuid().ToString("N"),
        AutoOffsetReset = AutoOffsetReset.Earliest,
        AllowAutoCreateTopics = true
      };

      using (var consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build())
      {
        try
        {
          consumer.Subscribe(someTopic);
          var result = consumer.Consume(millisecondsTimeout: 500);
          result.Message.Value.Should().Be(expectedValue);
        }
        catch (Exception exception)
        {
          Console.WriteLine(exception);
          throw;
        }
      }
    }

    private readonly KafkaSetupContainerAsyncFixture _fixture;
  }
}
