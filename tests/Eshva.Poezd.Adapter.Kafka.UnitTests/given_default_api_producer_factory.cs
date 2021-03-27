#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_default_api_producer_factory
  {
    [Fact]
    public async Task when_create_producer_it_should_create_producer()
    {
      var published = 0;

      var producerMock = new Mock<IProducer<int, string>>();
      producerMock
        .Setup(
          producer => producer.ProduceAsync(
            It.IsAny<string>(),
            It.IsAny<Message<int, string>>(),
            It.IsAny<CancellationToken>()))
        .Callback(() => published++)
        .Returns(() => Task.FromResult<DeliveryResult<int, string>>(new DeliveryReport<int, string>()));
      var producerFactoryMock = new Mock<IProducerFactory>();
      producerFactoryMock.Setup(factory => factory.Create<int, string>(It.IsAny<ProducerConfig>())).Returns(producerMock.Object);
      var loggerFactoryMock = new Mock<ILoggerFactory>();
      loggerFactoryMock.Setup(factory => factory.CreateLogger(It.IsAny<string>())).Returns(Mock.Of<ILogger>);

      var sut = new DefaultApiProducerFactory(
        producerFactoryMock.Object,
        Mock.Of<IHeaderValueCodec>(),
        loggerFactoryMock.Object);

      var apiProducer = sut.Create<int, string>(new ProducerConfig());
      var context = new MessagePublishingContext
      {
        Key = 555,
        Payload = "payload",
        Api = Mock.Of<IEgressApi>(),
        Metadata = new Dictionary<string, string>(),
        QueueNames = new[] {"topic1"},
        Broker = Mock.Of<IMessageBroker>()
      };

      await apiProducer.Publish(context, CancellationToken.None);

      published.Should().Be(expected: 1, "factory should create functional producer");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_construct_with_invalid_arguments_it_should_fail()
    {
      var producerFactory = Mock.Of<IProducerFactory>();
      var headerValueCodec = Mock.Of<IHeaderValueCodec>();
      var loggerFactory = Mock.Of<ILoggerFactory>();
      Action sut = () => new DefaultApiProducerFactory(
        producerFactory,
        headerValueCodec,
        loggerFactory);

      producerFactory = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      producerFactory = Mock.Of<IProducerFactory>();

      headerValueCodec = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      headerValueCodec = Mock.Of<IHeaderValueCodec>();

      loggerFactory = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_create_producer_with_invalid_arguments_it_should_fail()
    {
      var factory = new DefaultApiProducerFactory(
        Mock.Of<IProducerFactory>(),
        Mock.Of<IHeaderValueCodec>(),
        Mock.Of<ILoggerFactory>());

      Action sut = () => factory.Create<int, string>(config: null);

      sut.Should().ThrowExactly<ArgumentNullException>();
    }
  }
}
