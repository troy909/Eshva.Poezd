#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using FluentAssertions;
using Moq;
using RandomStringCreator;
using Venture.Common.TestingTools.Kafka;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.IntegrationTests
{
  [Collection(KafkaSetupCollection.Name)]
  public class given_default_producer_factory
  {
    public given_default_producer_factory(KafkaSetupContainerAsyncFixture fixture)
    {
      if (fixture == null) throw new ArgumentNullException(nameof(fixture));
      _kafkaTestContextFactory = new KafkaTestContextFactory(fixture.KafkaContainerConfiguration.BootstrapServers);
    }

    [Fact]
    public async Task when_create_producer_it_should_configure_it_using_configurator()
    {
      var configuratorMock = new Mock<IProducerConfigurator>();
      configuratorMock
        .Setup(
          configurator => configurator.Configure(
            It.IsAny<ProducerBuilder<string, byte[]>>(),
            It.IsAny<ISerializer<string>>(),
            It.IsAny<ISerializer<byte[]>>()))
        .Returns(
          (
            ProducerBuilder<string, byte[]> builder,
            ISerializer<string> keySerializer,
            ISerializer<byte[]> valueSerializer) =>
          {
            builder.SetKeySerializer(keySerializer);
            builder.SetValueSerializer(valueSerializer);
            return builder;
          })
        .Verifiable("configurator should be called");

      var serializerFactoryMock = new Mock<ISerializerFactory>();
      serializerFactoryMock.Setup(factory => factory.Create<string>()).Returns(Serializers.Utf8)
        .Verifiable("serializer for the key should be gotten");
      serializerFactoryMock.Setup(factory => factory.Create<byte[]>()).Returns(Serializers.ByteArray)
        .Verifiable("serializer for the value should be gotten");

      var producerFactory = new DefaultProducerFactory(configuratorMock.Object, serializerFactoryMock.Object);
      var producer = producerFactory.Create<string, byte[]>(CreateProducerConfig());

      var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
      await using var kafkaTestContext = _kafkaTestContextFactory.Create<string, byte[]>(timeout);
      var topic = new StringCreator().Get(length: 10);
      await kafkaTestContext.CreateTopics(topic);

      var key = StringCreator.Get(length: 10);
      var value = Encoding.UTF8.GetBytes("test-message");
      var message = new Message<string, byte[]> {Headers = new Headers(), Key = key, Timestamp = Timestamp.Default, Value = value};

      Func<Task> sut = () => producer.ProduceAsync(
        topic,
        message,
        timeout);

      sut.Should().NotThrow();
      configuratorMock.Verify();
      serializerFactoryMock.Verify();

      var consumeResult = kafkaTestContext.Consume(topic);
      consumeResult.Message.Key.Should().Be(key);
      consumeResult.Message.Value.Should().BeEquivalentTo(value);
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_constructed_with_invalid_arguments_it_should_fail()
    {
      var configurator = Mock.Of<IProducerConfigurator>();
      var serializerFactory = Mock.Of<ISerializerFactory>();

      Action sut = () => new DefaultProducerFactory(configurator, serializerFactory);

      configurator = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      configurator = Mock.Of<IProducerConfigurator>();

      serializerFactory = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_create_producer_with_invalid_arguments_it_should_fail()
    {
      var configurator = Mock.Of<IProducerConfigurator>();
      var serializerFactory = Mock.Of<ISerializerFactory>();

      var factory = new DefaultProducerFactory(configurator, serializerFactory);

      Action sut = () => factory.Create<int, string>(config: null);

      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    private static ProducerConfig CreateProducerConfig()
    {
      var config = new ProducerConfig
      {
        BootstrapServers = "localhost:9092",
        ApiVersionRequest = true,
        QueueBufferingMaxKbytes = 10240,
        MessageTimeoutMs = 3000
      };
      config.Set(@"request.required.acks", "-1");
      config.Set(@"queue.buffering.max.ms", "5");
      return config;
    }

    private readonly KafkaTestContextFactory _kafkaTestContextFactory;
    private static readonly StringCreator StringCreator = new StringCreator();
  }
}
