#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.UnitTests.Tools;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_broker_egress_kafka_driver
  {
    [Fact]
    public void when_constructing_without_configuration_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => { new BrokerEgressKafkaDriver(driverConfiguration: null, new Mock<IProducerRegistry>().Object); };
      sut.Should().ThrowExactly<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("driverConfiguration"),
        "configuration should be specified");
    }

    [Fact]
    public void when_constructing_without_registry_it_should_fail()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      // ReSharper disable once ObjectCreationAsStatement
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => new BrokerEgressKafkaDriver(configuration, producerRegistry: null);
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("producerRegistry"));
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_initializing_with_invalid_arguments_it_should_fail()
    {
      var publishedMessages = new List<MessagePublishingContext>();
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages),
        new Mock<IProducerRegistry>().Object);

      var apis = new IEgressApi[0];
      var serviceProvider = Mock.Of<IDiContainerAdapter>();

      Action sut = () => driver.Initialize(
        apis,
        serviceProvider);

      apis = null;
      sut.Should().ThrowExactly<ArgumentNullException>()
        .Which.ParamName.Should().Be("apis", "null is an invalid APIs");
      apis = new IEgressApi[0];

      serviceProvider = null;
      sut.Should().ThrowExactly<ArgumentNullException>()
        .Which.ParamName.Should().Be("serviceProvider", "null is an invalid service provider");
    }

    [Fact]
    public void when_initializing_twice_it_should_fail()
    {
      var publishedMessages = new List<MessagePublishingContext>();
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages),
        new Mock<IProducerRegistry>().Object);

      Action sut = () => driver.Initialize(
        new IEgressApi[0],
        MakeServiceProvider());
      sut.Should().NotThrow();
      sut.Should().ThrowExactly<PoezdOperationException>().Where(exception => exception.Message.Contains("already initialized"));
    }

    [Fact]
    public void when_publishing_to_not_initialized_driver_it_should_fail()
    {
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(),
        new Mock<IProducerRegistry>().Object);
      Func<Task> sut = () => driver.Publish(MakeContext(), CancellationToken.None);

      sut.Should().ThrowExactly<PoezdOperationException>()
        .Where(
          exception => exception.Message.Contains("initialized", StringComparison.InvariantCultureIgnoreCase),
          "not initialized driver can not publish messages");
    }

    [Fact]
    public void when_dispose_it_should_dispose_producer_registry()
    {
      var registryMock = new Mock<IProducerRegistry>();
      registryMock.Setup(registry => registry.Dispose()).Verifiable();
      var sut = new BrokerEgressKafkaDriver(new BrokerEgressKafkaDriverConfiguration(), registryMock.Object);
      sut.Dispose();
      registryMock.Verify();
    }

    [Fact]
    public async Task when_publish_using_initialized_driver_it_should_publish_to_api_publisher()
    {
      var published = 0;

      var apiProducerMock = new Mock<IApiProducer>();
      apiProducerMock
        .Setup(producer => producer.Publish(It.IsAny<MessagePublishingContext>(), It.IsAny<CancellationToken>()))
        .Callback(() => published++)
        .Returns(() => Task.CompletedTask);

      var producerRegistryMock = new Mock<IProducerRegistry>();
      producerRegistryMock.Setup(registry => registry.Get(It.IsAny<IEgressApi>())).Returns(() => apiProducerMock.Object);

      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(new List<MessagePublishingContext>()),
        producerRegistryMock.Object);

      driver.Initialize(
        new[] {MakeApi()},
        MakeServiceProvider());

      await driver.Publish(MakeContext(), CancellationToken.None);
      published.Should().Be(expected: 1, "message should be published");
    }

    [Fact]
    public void when_publish_with_invalid_arguments_it_should_fail()
    {
      var apiProducerMock = new Mock<IApiProducer>();
      apiProducerMock
        .Setup(producer => producer.Publish(It.IsAny<MessagePublishingContext>(), It.IsAny<CancellationToken>()))
        .Returns(() => Task.CompletedTask);

      var producerRegistryMock = new Mock<IProducerRegistry>();
      producerRegistryMock.Setup(registry => registry.Get(It.IsAny<IEgressApi>())).Returns(() => apiProducerMock.Object);

      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(new List<MessagePublishingContext>()),
        producerRegistryMock.Object);

      driver.Initialize(
        new[] {MakeApi()},
        MakeServiceProvider());

      // ReSharper disable once AssignNullToNotNullAttribute
      Func<Task> sut = () => driver.Publish(context: null, CancellationToken.None);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    private static IEgressApi MakeApi()
    {
      var descriptorMock = new Mock<IEgressApiMessageTypeDescriptor<It.IsAnyType>>();
      descriptorMock.Setup(descriptor => descriptor.QueueNames).Returns(() => new[] {"topic1"});
      var typesRegistryMock = new Mock<IEgressApiMessageTypesRegistry>();
      typesRegistryMock.Setup(registry => registry.DoesOwn<It.IsAnyType>()).Returns(value: true);
      typesRegistryMock.Setup(registry => registry.GetDescriptorByMessageType<It.IsAnyType>()).Returns(() => descriptorMock.Object);
      var apiMock = new Mock<IEgressApi>();
      apiMock.SetupGet(api => api.MessageTypesRegistry).Returns(() => typesRegistryMock.Object);
      apiMock.SetupGet(api => api.MessageKeyType).Returns(() => typeof(int));
      apiMock.SetupGet(api => api.MessagePayloadType).Returns(() => typeof(string));
      return apiMock.Object;
    }

    private static MessagePublishingContext MakeContext() =>
      new MessagePublishingContext
      {
        Key = "key",
        Payload = "payload",
        Api = Mock.Of<IEgressApi>(),
        Metadata = new Dictionary<string, string>(),
        QueueNames = new string[0],
        Broker = Mock.Of<IMessageBroker>()
      };

    private static IDiContainerAdapter MakeServiceProvider()
    {
      var mock = new Mock<IDiContainerAdapter>();
      mock
        .Setup(adapter => adapter.GetService(typeof(ConfigurationTests.SerializerFactory)))
        .Returns(() => new ConfigurationTests.SerializerFactory());
      mock
        .Setup(adapter => adapter.GetService(typeof(ConfigurationTests.HeaderValueCodec)))
        .Returns(() => new ConfigurationTests.HeaderValueCodec());
      mock
        .Setup(adapter => adapter.GetService(typeof(ConfigurationTests.ProducerConfigurator)))
        .Returns(() => new ConfigurationTests.ProducerConfigurator());
      mock
        .Setup(adapter => adapter.GetService(typeof(ConfigurationTests.ProducerFactory)))
        .Returns(() => new ConfigurationTests.ProducerFactory());
      mock
        .Setup(adapter => adapter.GetService(typeof(ConfigurationTests.ApiProducerFactory)))
        .Returns(() => new ConfigurationTests.ApiProducerFactory());
      mock
        .Setup(adapter => adapter.GetService(typeof(ILoggerFactory)))
        .Returns(Mock.Of<ILoggerFactory>());
      return mock.Object;
    }
  }
}
