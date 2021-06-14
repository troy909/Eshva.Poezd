#region Usings

using System;
using System.Text;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Common.Testing;
using Eshva.Common.Tpl;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using FluentAssertions;
using Moq;
using Venture.Common.TestingTools.Kafka;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public class given_default_consumer_factory
  {
    public given_default_consumer_factory(KafkaSetupContainerAsyncFixture fixture)
    {
      if (fixture == null) throw new ArgumentNullException(nameof(fixture));
      _kafkaTestContextFactory = new KafkaTestContextFactory(fixture.KafkaContainerConfiguration.BootstrapServers);
    }

    [Fact]
    public async Task when_create_consumer_it_should_configure_it_using_configurator()
    {
      var consumerFactory = new DefaultConsumerFactory();
      var configuratorMock = new Mock<IConsumerConfigurator>();
      configuratorMock
        .Setup(
          configurator => configurator.Configure(
            It.IsAny<ConsumerBuilder<int, byte[]>>(),
            It.IsAny<IDeserializer<int>>(),
            It.IsAny<IDeserializer<byte[]>>()))
        .Returns(
          (
            ConsumerBuilder<int, byte[]> builder,
            IDeserializer<int> keyDeserializer,
            IDeserializer<byte[]> valueDeserializer) =>
          {
            builder.SetKeyDeserializer(keyDeserializer);
            builder.SetValueDeserializer(valueDeserializer);
            return builder;
          })
        .Verifiable("configurator should be called");

      var deserializerFactoryMock = new Mock<IDeserializerFactory>();
      deserializerFactoryMock.Setup(factory => factory.Create<int>()).Returns(Deserializers.Int32)
        .Verifiable("deserializer for the key should be gotten");
      deserializerFactoryMock.Setup(factory => factory.Create<byte[]>()).Returns(Deserializers.ByteArray)
        .Verifiable("deserializer for the value should be gotten");

      using var consumer = consumerFactory.Create<int, byte[]>(
        CreateConsumerConfig(),
        configuratorMock.Object,
        deserializerFactoryMock.Object);

      var timeout = Cancellation.TimeoutToken(TimeSpan.FromSeconds(value: 5));
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<int, byte[]>(timeout);
      var topic = Randomize.String(length: 10);
      await kafkaTestContext.CreateTopics(topic);
      var key = 123;
      var value = Encoding.UTF8.GetBytes("test-message");
      await kafkaTestContext.Produce(
        topic,
        key,
        value);

      consumer.Subscribe(topic);
      // ReSharper disable once AccessToDisposedClosure
      Action sut = () => consumer.Consume(timeout);

      sut.Should().NotThrow();
      configuratorMock.Verify();
      deserializerFactoryMock.Verify();
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

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
  }
}
