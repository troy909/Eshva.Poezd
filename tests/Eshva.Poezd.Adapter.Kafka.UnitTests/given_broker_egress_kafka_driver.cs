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
using Eshva.Poezd.Adapter.Kafka.UnitTests.Tools;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
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
      Action sut = () => new BrokerEgressKafkaDriver(configuration: null);
      sut.Should().ThrowExactly<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("configuration"),
        "configuration should be specified");
    }

    [Fact]
    public void when_publishing_to_not_initialized_driver_it_should_fail()
    {
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration());
      Func<Task> sut = () => driver.Publish(
        "key",
        "payload",
        new Dictionary<string, string>(),
        new string[0],
        CancellationToken.None);

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
      var publishedMessages = new Dictionary<string, object>();
      var driver = new BrokerEgressKafkaDriver(ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages));
      driver.Initialize(
        brokerId,
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow));
      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");
      const string topic1 = "commands1";
      const string topic2 = "audit2";
      const string headerKey = "Type";
      const string headerValue = "Message1";
      await driver.Publish(
        key,
        payload,
        new Dictionary<string, string> {{headerKey, headerValue}},
        new[] {topic1, topic2},
        CancellationToken.None);
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
      var publishedMessages = new Dictionary<string, object>();
      var driver = new BrokerEgressKafkaDriver(ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages));
      driver.Initialize(
        brokerId,
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow));
      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");
      const string topic1 = "commands1";
      const string topic2 = "audit2";
      await driver.Publish(
        key,
        payload,
        new Dictionary<string, string> {{"Type", "Message1"}},
        new[] {topic1, topic2},
        CancellationToken.None);
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
      var publishedMessages = new Dictionary<string, object>();
      const string exceptionMessage = "Exception message";
      var driver = new BrokerEgressKafkaDriver(
        ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages, new IOException("Exception message")));
      driver.Initialize(
        brokerId,
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow));
      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");
      const string topic1 = "commands1";
      const string topic2 = "audit2";
      Func<Task> sut = () => driver.Publish(
        key,
        payload,
        new Dictionary<string, string> {{"Type", "Message1"}},
        new[] {topic1, topic2},
        CancellationToken.None);

      sut.Should().ThrowExactly<IOException>().Where(exception => exception.Message.Equals(exceptionMessage));

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
    public void when_initializing_with_invalid_arguments_it_should_fail()
    {
      var container = new Container().AddLogging(_testOutput);
      var publishedMessages = new Dictionary<string, object>();
      var driver = new BrokerEgressKafkaDriver(ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages));

      var brokerId = "broker-1";
      var logger = container.GetInstance<ILogger<IBrokerEgressDriver>>();
      var clock = new TestClock(DateTimeOffset.UtcNow);

      Action sut = () => driver.Initialize(
        brokerId,
        logger,
        clock);

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
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_publishing_with_invalid_arguments_it_should_fail()
    {
      var container = new Container().AddLogging(_testOutput);
      var publishedMessages = new Dictionary<string, object>();
      var driver = new BrokerEgressKafkaDriver(ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages));
      driver.Initialize(
        "broker-1",
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow));

      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");

      var metadata = new Dictionary<string, string> {{"Type", "Message1"}};
      var queueNames = new[] {"commands1", "audit2"};
      Func<Task> sut = () => driver.Publish(
        key,
        payload,
        metadata,
        queueNames,
        CancellationToken.None);

      metadata = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("metadata", "null is an invalid metadata");

      metadata = new Dictionary<string, string> {{"Type", "Message1"}};
      queueNames = null;
      sut.Should().ThrowExactly<ArgumentNullException>()
        .Which.ParamName.Should().Be("queueNames", "null is an invalid queue names");
    }

    [Fact]
    public void when_publishing_cancelled_it_should_report_about_it()
    {
      var container = new Container().AddLogging(_testOutput);
      var publishedMessages = new Dictionary<string, object>();
      var driver = new BrokerEgressKafkaDriver(ConfigurationTests.CreateBrokerEgressKafkaDriverConfiguration(publishedMessages));
      driver.Initialize(
        "broker-1",
        container.GetInstance<ILogger<IBrokerEgressDriver>>(),
        new TestClock(DateTimeOffset.UtcNow));

      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");

      var metadata = new Dictionary<string, string> {{"Type", "Message1"}};
      var queueNames = new[] {"commands1", "audit2"};
      var cancelledToken = new CancellationToken(canceled: true);
      driver.Publish(
        key,
        payload,
        metadata,
        queueNames,
        cancelledToken).IsCanceled.Should().BeTrue("publishing was cancelled");
    }

    private readonly ITestOutputHelper _testOutput;
    private const string WhitespaceString = "\t\n ";
  }
}
