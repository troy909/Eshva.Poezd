#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_default_api_consumer_factory
  {
    [Fact]
    public void when_create_consumer_it_should_create_consumer()
    {
      var created = 0;

      var consumerFactoryMock = new Mock<IConsumerFactory>();
      consumerFactoryMock
        .Setup(
          factory => factory.Create<int, string>(
            It.IsAny<ConsumerConfig>(),
            It.IsAny<IConsumerConfigurator>(),
            It.IsAny<IDeserializerFactory>()))
        .Callback(() => created++)
        .Returns(Mock.Of<IConsumer<int, string>>);

      var sut = new DefaultApiConsumerFactory(
        consumerFactoryMock.Object,
        Mock.Of<IConsumerConfigurator>(),
        Mock.Of<IDeserializerFactory>(),
        Mock.Of<ILoggerFactory>());

      sut.Create<int, string>(new ConsumerConfig(), Mock.Of<IIngressApi>());
      created.Should().Be(expected: 1, "one consumer should be created");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_create_consumer_with_invalid_argument_it_should_fail()
    {
      var factory = new DefaultApiConsumerFactory(
        Mock.Of<IConsumerFactory>(),
        Mock.Of<IConsumerConfigurator>(),
        Mock.Of<IDeserializerFactory>(),
        Mock.Of<ILoggerFactory>());
      var config = new ConsumerConfig();
      var api = Mock.Of<IIngressApi>();

      Action sut = () => factory.Create<int, string>(config, api);

      config = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      config = new ConsumerConfig();

      api = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_constructed_with_invalid_arguments_it_should_fail()
    {
      var consumerFactory = Mock.Of<IConsumerFactory>();
      var consumerConfigurator = Mock.Of<IConsumerConfigurator>();
      var deserializerFactory = Mock.Of<IDeserializerFactory>();
      var loggerFactory = Mock.Of<ILoggerFactory>();

      Action sut = () => new DefaultApiConsumerFactory(
        consumerFactory,
        consumerConfigurator,
        deserializerFactory,
        loggerFactory);

      consumerFactory = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      consumerFactory = Mock.Of<IConsumerFactory>();

      consumerConfigurator = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      consumerConfigurator = Mock.Of<IConsumerConfigurator>();

      deserializerFactory = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      deserializerFactory = Mock.Of<IDeserializerFactory>();

      loggerFactory = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
    }
  }
}
