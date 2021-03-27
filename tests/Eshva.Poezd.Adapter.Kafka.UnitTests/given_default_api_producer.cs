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
  public class given_default_api_producer
  {
    public given_default_api_producer(ITestOutputHelper testOutput)
    {
      _testOutput = testOutput;
    }

    [Fact]
    public async Task when_publishing_to_properly_initialized_driver_it_should_publish_message_to_producer()
    {
      var container = new Container().AddLogging(_testOutput);
      var publishedMessages = new Dictionary<string, Message<int, byte[]>>();

      var producerMock = new Mock<IProducer<int, byte[]>>();
      producerMock
        .Setup(
          producer => producer.ProduceAsync(
            It.IsAny<string>(),
            It.IsAny<Message<int, byte[]>>(),
            CancellationToken.None))
        .Callback(
          (
            string topic,
            Message<int, byte[]> message,
            CancellationToken token) => publishedMessages.Add(topic, message))
        .Returns(() => Task.FromResult(new DeliveryResult<int, byte[]>()));

      var apiProducer = new DefaultApiProducer<int, byte[]>(
        producerMock.Object,
        new Utf8ByteStringHeaderValueCodec(),
        container.GetInstance<ILogger<DefaultApiProducer<int, byte[]>>>());

      const int key = 555;
      var payload = Encoding.UTF8.GetBytes("payload");
      const string headerKey = "Type";
      const string headerValue = "Message1";
      const string topic1 = "commands1";
      const string topic2 = "audit2";

      var context = new MessagePublishingContext
      {
        Key = key,
        Payload = payload,
        Api = Mock.Of<IEgressApi>(),
        Metadata = new Dictionary<string, string> {{headerKey, headerValue}},
        QueueNames = new[] {topic1, topic2},
        Broker = Mock.Of<IMessageBroker>()
      };

      await apiProducer.Publish(context, CancellationToken.None);

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
    public void when_publish_with_invalid_arguments_it_should_fail()
    {
      var apiProducer = new DefaultApiProducer<string, string>(
        Mock.Of<IProducer<string, string>>(),
        Mock.Of<IHeaderValueCodec>(),
        Mock.Of<ILogger<DefaultApiProducer<string, string>>>());

      // ReSharper disable once AssignNullToNotNullAttribute
      Func<Task> sut = () => apiProducer.Publish(context: null, CancellationToken.None);

      sut.Should().ThrowExactly<ArgumentNullException>("null is not valid context");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_publishing_with_invalid_context_content_it_should_fail()
    {
      var apiProducer = new DefaultApiProducer<string, string>(
        Mock.Of<IProducer<string, string>>(),
        Mock.Of<IHeaderValueCodec>(),
        Mock.Of<ILogger<DefaultApiProducer<string, string>>>());

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

      Func<Task> sut = () => apiProducer.Publish(context, CancellationToken.None);

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
      var producer = new TestProducer<int, byte[]>(new ProducerConfig(), new Dictionary<string, object>());
      var sut = new DefaultApiProducer<int, byte[]>(
        producer,
        Mock.Of<IHeaderValueCodec>(),
        Mock.Of<ILogger<DefaultApiProducer<int, byte[]>>>());

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

      var publish = sut.Publish(context, cancelledToken);
      publish.IsCanceled.Should().BeTrue("publishing was cancelled");
    }

    [Fact]
    public async Task when_publishing_to_properly_initialized_driver_it_should_publish_messages_and_log()
    {
      var container = new Container().AddLogging(_testOutput);
      const string brokerId = "broker-1";
      var brokerMock = new Mock<IMessageBroker>();
      brokerMock.SetupGet(broker => broker.Id).Returns(brokerId);

      var producer = new TestProducer<int, byte[]>(new ProducerConfig(), new Dictionary<string, object>());
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

      var apiProducer = new DefaultApiProducer<int, byte[]>(
        producer,
        Mock.Of<IHeaderValueCodec>(),
        container.GetInstance<ILogger<DefaultApiProducer<int, byte[]>>>());

      await apiProducer.Publish(context, CancellationToken.None);

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

      var producer = new TestProducer<int, byte[]>(
        new ProducerConfig(),
        new Dictionary<string, object>(),
        new IOException());
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

      var apiProducer = new DefaultApiProducer<int, byte[]>(
        producer,
        Mock.Of<IHeaderValueCodec>(),
        container.GetInstance<ILogger<DefaultApiProducer<int, byte[]>>>());

      Func<Task> sut = () => apiProducer.Publish(context, CancellationToken.None);

      sut.Should().ThrowExactly<IOException>("should fail if publishing failed");

      InMemorySink.Instance.LogEvents.Should().HaveCount(2 * 2, "message publishing failed in both topics");
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
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_construct_with_invalid_arguments_it_should_fail()
    {
      var producer = Mock.Of<IProducer<int, string>>();
      var headerValueCodec = Mock.Of<IHeaderValueCodec>();
      var logger = new Mock<ILogger<DefaultApiProducer<int, string>>>().Object;
      Action sut = () => new DefaultApiProducer<int, string>(
        producer,
        headerValueCodec,
        logger);

      producer = null;
      sut.Should().ThrowExactly<ArgumentNullException>("null is invalid producer");
      producer = Mock.Of<IProducer<int, string>>();

      headerValueCodec = null;
      sut.Should().ThrowExactly<ArgumentNullException>("null is invalid header value codec");
      headerValueCodec = Mock.Of<IHeaderValueCodec>();

      logger = null;
      sut.Should().ThrowExactly<ArgumentNullException>("null is invalid logger");
    }

    [Fact]
    public void when_dispose_it_should_dispose_producer()
    {
      var producerMock = new Mock<IProducer<int, string>>();
      producerMock.Setup(producer => producer.Dispose()).Verifiable("producer should be disposed");
      var sut = new DefaultApiProducer<int, string>(
        producerMock.Object,
        Mock.Of<IHeaderValueCodec>(),
        new Mock<ILogger<DefaultApiProducer<int, string>>>().Object);
      sut.Dispose();
    }

    private readonly ITestOutputHelper _testOutput;
  }
}
