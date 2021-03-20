#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Common.Testing;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.UnitTests.Tools;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_broker_egress_kafka_driver
  {
    public given_broker_egress_kafka_driver(ITestOutputHelper testOutput)
    {
      _testOutput = testOutput;
    }

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
      var container = new Container().AddLogging(_testOutput);
      var publishedMessages = new Dictionary<string, object>();
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages),
        new Mock<IProducerRegistry>().Object);

      var brokerId = "broker-1";
      var logger = container.GetInstance<ILogger<IBrokerEgressDriver>>();
      var clock = Mock.Of<IClock>();

      Action sut = () => driver.Initialize(
        brokerId,
        logger,
        clock,
        new IEgressApi[0],
        Mock.Of<IDiContainerAdapter>());

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
      logger = null;
      sut.Should().ThrowExactly<ArgumentNullException>()
        .Which.ParamName.Should().Be("logger", "null is an invalid logger");

      logger = container.GetInstance<ILogger<IBrokerEgressDriver>>();
      clock = null;
      sut.Should().ThrowExactly<ArgumentNullException>()
        .Which.ParamName.Should().Be("clock", "null is an invalid clock");
    }

    [Fact]
    public void when_initializing_twice_it_should_fail()
    {
      var container = new Container().AddLogging(_testOutput);
      var publishedMessages = new Dictionary<string, object>();
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages),
        new Mock<IProducerRegistry>().Object);

      Action sut = () => driver.Initialize(
        "broker-1",
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow),
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
    public async Task when_publishing_to_properly_initialized_driver_it_should_publish_message_to_producer()
    {
      var container = new Container().AddLogging(_testOutput);
      const string brokerId = "broker-1";
      var brokerMock = new Mock<IMessageBroker>();
      brokerMock.SetupGet(broker => broker.Id).Returns(brokerId);
      var publishedMessages = new Dictionary<string, object>();

      var registryMock = new Mock<IProducerRegistry>();
      registryMock.Setup(registry => registry.Get<int, byte[]>(It.IsAny<IEgressApi>()))
        .Returns(new TestProducer<int, byte[]>(new ProducerConfig(), publishedMessages));

      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages),
        registryMock.Object);
      driver.Initialize(
        brokerId,
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow),
        new IEgressApi[0],
        Mock.Of<IDiContainerAdapter>());
      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");
      const string topic1 = "commands1";
      const string topic2 = "audit2";
      const string headerKey = "Type";
      const string headerValue = "Message1";
      var context = new MessagePublishingContext
      {
        Key = key,
        Payload = payload,
        Api = Mock.Of<IEgressApi>(),
        Metadata = new Dictionary<string, string> {{headerKey, headerValue}},
        QueueNames = new[] {topic1, topic2},
        Broker = brokerMock.Object
      };

      await driver.Publish(context, CancellationToken.None);
      publishedMessages.Should().HaveCount(expected: 2, "published 1 message to 2 topics");
      publishedMessages.ElementAt(index: 0).Key.Should().Be(topic1, "should publish to topics in order of occurrence");
      publishedMessages.ElementAt(index: 0).Value.As<Message<int, byte[]>>().Key.Should().Be(key, "key should have expected value");
      publishedMessages.ElementAt(index: 0).Value.As<Message<int, byte[]>>().Value.Should()
        .Equal(payload, "payload should have expected value");
      var header = publishedMessages.ElementAt(index: 0).Value.As<Message<int, byte[]>>().Headers.Single();
      header.Key.Should().Be(headerKey, "single header should have expected value");
      header.GetValueBytes().Should().Equal(Encoding.UTF8.GetBytes(headerValue), "single header should have expected value");

      publishedMessages.ElementAt(index: 1).Key.Should().Be(topic2, "should publish to topics in order of occurrence");
      publishedMessages.ElementAt(index: 1).Value.As<Message<int, byte[]>>().Key.Should().Be(key, "key should have expected value");
      publishedMessages.ElementAt(index: 1).Value.As<Message<int, byte[]>>().Value.Should()
        .Equal(payload, "payload should have expected value");
      header = publishedMessages.ElementAt(index: 1).Value.As<Message<int, byte[]>>().Headers.Single();
      header.Key.Should().Be(headerKey, "single header should have expected value");
      header.GetValueBytes().Should().Equal(Encoding.UTF8.GetBytes(headerValue), "single header should have expected value");
    }

    [Fact]
    public async Task when_publishing_to_properly_initialized_driver_it_should_publish_messages_and_log()
    {
      var container = new Container().AddLogging(_testOutput);
      const string brokerId = "broker-1";

      var producerMock = new Mock<IProducer<int, byte[]>>();
      producerMock.Setup(
          producer => producer.ProduceAsync(
            It.IsAny<string>(),
            It.IsAny<Message<int, byte[]>>(),
            CancellationToken.None))
        .Returns(() => Task.FromResult(new DeliveryResult<int, byte[]>()));
      var registryMock = new Mock<IProducerRegistry>();
      registryMock.Setup(registry => registry.Get<int, byte[]>(It.IsAny<IEgressApi>()))
        .Returns(producerMock.Object);

      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(new Dictionary<string, object>()),
        registryMock.Object);
      driver.Initialize(
        brokerId,
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow),
        new IEgressApi[0],
        Mock.Of<IDiContainerAdapter>());
      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");
      const string topic1 = "commands1";
      const string topic2 = "audit2";
      var brokerMock = new Mock<IMessageBroker>();
      brokerMock.SetupGet(broker => broker.Id).Returns(brokerId);
      var context = new MessagePublishingContext
      {
        Key = key,
        Payload = payload,
        Api = Mock.Of<IEgressApi>(),
        Metadata = new Dictionary<string, string> {{"Type", "Message1"}},
        QueueNames = new[] {topic1, topic2},
        Broker = brokerMock.Object
      };

      await driver.Publish(context, CancellationToken.None);
      InMemorySink.Instance.LogEvents.Should().HaveCount(2 * 2, "message was published to 2 topics");
      InMemorySink.Instance.LogEvents.ElementAt(index: 0).RenderMessage().Should().Contain("Publishing")
        .And.Contain(key.ToString())
        .And.Contain(topic1)
        .And.Contain(brokerId);
      InMemorySink.Instance.LogEvents.ElementAt(index: 1).RenderMessage().Should().Contain("Successfully")
        .And.Contain(key.ToString())
        .And.Contain(topic1)
        .And.Contain(brokerId);
      InMemorySink.Instance.LogEvents.ElementAt(index: 2).RenderMessage().Should().Contain("Publishing")
        .And.Contain(key.ToString())
        .And.Contain(topic2)
        .And.Contain(brokerId);
      InMemorySink.Instance.LogEvents.ElementAt(index: 3).RenderMessage().Should().Contain("Successfully")
        .And.Contain(key.ToString())
        .And.Contain(topic2)
        .And.Contain(brokerId);
    }

    [Fact]
    public void when_publishing_and_error_happens_it_should_log_error_and_fail()
    {
      var container = new Container().AddLogging(_testOutput);
      const string brokerId = "broker-1";
      var brokerMock = new Mock<IMessageBroker>();
      brokerMock.SetupGet(broker => broker.Id).Returns(brokerId);

      var producerMock = new Mock<IProducer<int, byte[]>>();
      producerMock.Setup(
          producer => producer.ProduceAsync(
            It.IsAny<string>(),
            It.IsAny<Message<int, byte[]>>(),
            CancellationToken.None))
        .Throws<IOException>();
      var registryMock = new Mock<IProducerRegistry>();
      registryMock.Setup(registry => registry.Get<int, byte[]>(It.IsAny<IEgressApi>()))
        .Returns(producerMock.Object);

      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(
          new Dictionary<string, object>(),
          new IOException("Exception message")),
        registryMock.Object);
      driver.Initialize(
        brokerId,
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow),
        new IEgressApi[0],
        Mock.Of<IDiContainerAdapter>());
      const int key = 555;
      const string topic1 = "commands1";
      const string topic2 = "audit2";
      var context = new MessagePublishingContext
      {
        Key = key,
        Payload = Encoding.UTF8.GetBytes("payload"),
        Api = Mock.Of<IEgressApi>(),
        Metadata = new Dictionary<string, string> {{"Type", "Message1"}},
        QueueNames = new[] {topic1, topic2},
        Broker = brokerMock.Object
      };

      Func<Task> sut = () => driver.Publish(context, CancellationToken.None);

      sut.Should().ThrowExactly<IOException>("should fail if publishing failed");

      InMemorySink.Instance.LogEvents.Should().HaveCount(1 * 2, "message publishing failed in the first topic");
      InMemorySink.Instance.LogEvents.ElementAt(index: 0).RenderMessage().Should().Contain("Publishing")
        .And.Contain(key.ToString())
        .And.Contain(topic1)
        .And.Contain(brokerId);
      InMemorySink.Instance.LogEvents.ElementAt(index: 1).RenderMessage().Should().Contain("Failed")
        .And.Contain(key.ToString())
        .And.Contain(topic1)
        .And.Contain(brokerId);
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_publishing_with_invalid_arguments_it_should_fail()
    {
      var container = new Container().AddLogging(_testOutput);
      var publishedMessages = new Dictionary<string, object>();
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages),
        new Mock<IProducerRegistry>().Object);
      driver.Initialize(
        "broker-1",
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow),
        new IEgressApi[0],
        Mock.Of<IDiContainerAdapter>());

      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");

      var metadata = new Dictionary<string, string> {{"Type", "Message1"}};
      var queueNames = new[] {"commands1", "audit2"};
      var broker = Mock.Of<IMessageBroker>();
      var api = Mock.Of<IEgressApi>();
      var context = new MessagePublishingContext
      {
        Key = key,
        Payload = payload,
        Api = api,
        Metadata = metadata,
        QueueNames = queueNames,
        Broker = broker
      };

      Func<Task> sut = () => driver.Publish(context, CancellationToken.None);

      context.Metadata = null;
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("metadata", StringComparison.InvariantCultureIgnoreCase),
        "null is an invalid metadata");

      context.Metadata = metadata;
      context.QueueNames = null;
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("queueNames", StringComparison.InvariantCultureIgnoreCase),
        "null is an invalid queue names");

      context.QueueNames = queueNames;
      context.Key = null;
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("key", StringComparison.InvariantCultureIgnoreCase),
        "null is an invalid key");

      context.Key = key;
      context.Payload = null;
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("payload", StringComparison.InvariantCultureIgnoreCase),
        "null is an invalid payload");

      context.Payload = payload;
      context.Broker = null;
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("broker", StringComparison.InvariantCultureIgnoreCase),
        "null is an invalid broker");

      context.Broker = broker;
      context.Api = null;
      sut.Should().ThrowExactly<PoezdOperationException>().Where(
        exception => exception.Message.Contains("API", StringComparison.InvariantCultureIgnoreCase),
        "null is an invalid API");
    }

    [Fact]
    public void when_publishing_cancelled_it_should_report_about_it()
    {
      var container = new Container().AddLogging(_testOutput);
      var publishedMessages = new Dictionary<string, object>();

      var registryMock = new Mock<IProducerRegistry>();
      registryMock.Setup(registry => registry.Get<int, byte[]>(It.IsAny<IEgressApi>()))
        .Returns(new TestProducer<int, byte[]>(new ProducerConfig(), publishedMessages));

      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages),
        registryMock.Object);
      driver.Initialize(
        "broker-1",
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow),
        new IEgressApi[0],
        Mock.Of<IDiContainerAdapter>());

      var cancelledToken = new CancellationToken(canceled: true);
      var context = new MessagePublishingContext
      {
        Key = 555,
        Payload = Encoding.UTF8.GetBytes("payload"),
        Api = Mock.Of<IEgressApi>(),
        Metadata = new Dictionary<string, string> {{"Type", "Message1"}},
        QueueNames = new[] {"commands1", "audit2"},
        Broker = Mock.Of<IMessageBroker>()
      };

      driver.Publish(context, cancelledToken).IsCanceled.Should().BeTrue("publishing was cancelled");
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

    private readonly ITestOutputHelper _testOutput;
    private const string WhitespaceString = "\t\n ";
  }
}
