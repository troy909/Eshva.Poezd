#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.UnitTests.Tools;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
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

      var brokerId = "broker-1";
      var apis = new IEgressApi[0];
      var serviceProvider = Mock.Of<IDiContainerAdapter>();

      Action sut = () => driver.Initialize(
        brokerId,
        apis,
        serviceProvider);

      brokerId = null;
      sut.Should().ThrowExactly<ArgumentNullException>()
        .Which.ParamName.Should().Be("brokerId", "null is an invalid brokerId");
      brokerId = string.Empty;
      sut.Should().ThrowExactly<ArgumentNullException>()
        .Which.ParamName.Should().Be("brokerId", "an empty string is an invalid brokerId");
      brokerId = WhitespaceString;
      sut.Should().ThrowExactly<ArgumentNullException>()
        .Which.ParamName.Should().Be("brokerId", "a whitespace string is an invalid brokerId");
      brokerId = "broker-1";

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
        "broker-1",
        new IEgressApi[0],
        Mock.Of<IDiContainerAdapter>());
      sut.Should().NotThrow();
      sut.Should().ThrowExactly<PoezdOperationException>().Where(exception => exception.Message.Contains("already initialized"));
    }

    [Fact]
    public void when_publishing_to_not_initialized_driver_it_should_fail()
    {
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(),
        new Mock<IProducerRegistry>().Object);
      Func<Task> sut = () =>
      {
        var context = new MessagePublishingContext
        {
          Key = "key",
          Payload = "payload",
          Api = Mock.Of<IEgressApi>(),
          Metadata = new Dictionary<string, string>(),
          QueueNames = new string[0],
          Broker = Mock.Of<IMessageBroker>()
        };
        return driver.Publish(context, CancellationToken.None);
      };

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

    private const string WhitespaceString = "\t\n ";
  }
}
