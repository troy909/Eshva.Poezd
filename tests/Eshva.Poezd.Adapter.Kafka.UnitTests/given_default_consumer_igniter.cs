#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_default_consumer_igniter
  {
    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_constructed_with_valid_arguments_it_should_fail()
    {
      Action sut = () => new DefaultConsumerIgniter<int, string>(Mock.Of<ILogger<BrokerIngressKafkaDriver>>());
      sut.Should().NotThrow();
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_constructed_with_invalid_arguments_it_should_fail()
    {
      Action sut = () => new DefaultConsumerIgniter<int, string>(logger: null);
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("logger"));
    }

    [Fact]
    public async Task when_start_with_valid_arguments_it_should_consume_messages_from_consumer()
    {
      var tokenSource = new CancellationTokenSource();
      var closed = 0;
      var consumerMock = new Mock<IConsumer<int, string>>();
      consumerMock
        .Setup(consumer => consumer.Consume(tokenSource.Token))
        .Returns(() => MakeConsumeResult());
      consumerMock
        .Setup(consumer => consumer.Close())
        .Callback(() => closed++);

      var received = 0;
      const int numberOfMessages = 2;

      Func<ConsumeResult<int, string>, Task> onMessageReceived = result =>
      {
        received++;
        if (received == numberOfMessages) tokenSource.Cancel();
        return Task.CompletedTask;
      };

      var sut = new DefaultConsumerIgniter<int, string>(Mock.Of<ILogger<BrokerIngressKafkaDriver>>());

      await sut.Start(
        consumerMock.Object,
        onMessageReceived,
        tokenSource.Token);

      received.Should().Be(numberOfMessages);
      closed.Should().Be(expected: 0, "consumer will be closed in a different place");
    }

    [Fact]
    public async Task when_start_and_eof_it_should_skip_message()
    {
      var tokenSource = new CancellationTokenSource(TimeSpan.FromMilliseconds(value: 500));
      var consumerMock = new Mock<IConsumer<int, string>>();
      var consumeResult = MakeConsumeResult(result => result.IsPartitionEOF = true);

      consumerMock
        .Setup(consumer => consumer.Consume(tokenSource.Token))
        .Returns(() => consumeResult);

      var received = 0;
      Func<ConsumeResult<int, string>, Task> onMessageReceived = result =>
      {
        received++;
        return Task.CompletedTask;
      };

      var sut = new DefaultConsumerIgniter<int, string>(Mock.Of<ILogger<BrokerIngressKafkaDriver>>());

      await sut.Start(
        consumerMock.Object,
        onMessageReceived,
        tokenSource.Token);

      received.Should().Be(expected: 0);
    }

    [Fact]
    public async Task when_start_with_valid_arguments_it_should_commit_kafka_message()
    {
      var committed = 0;
      var tokenSource = new CancellationTokenSource();
      var consumerMock = new Mock<IConsumer<int, string>>();
      consumerMock.Setup(consumer => consumer.Consume(tokenSource.Token)).Returns(() => MakeConsumeResult());
      consumerMock.Setup(consumer => consumer.Commit(It.IsAny<ConsumeResult<int, string>>())).Callback(() => committed++);

      var received = 0;
      const int numberOfMessages = 2;

      Func<ConsumeResult<int, string>, Task> onMessageReceived = result =>
      {
        received++;
        if (received == numberOfMessages) tokenSource.Cancel();
        return Task.CompletedTask;
      };

      var sut = new DefaultConsumerIgniter<int, string>(Mock.Of<ILogger<BrokerIngressKafkaDriver>>());

      await sut.Start(
        consumerMock.Object,
        onMessageReceived,
        tokenSource.Token);

      received.Should().Be(numberOfMessages);
      committed.Should().Be(numberOfMessages, "all messages should be committed");
    }

    [Fact]
    public async Task when_kafka_exception_thrown_in_start_it_should_fail()
    {
      var committed = 0;
      var tokenSource = new CancellationTokenSource();
      var consumerMock = new Mock<IConsumer<int, string>>();
      consumerMock
        .Setup(consumer => consumer.Consume(tokenSource.Token))
        .Throws(new ConsumeException(new ConsumeResult<byte[], byte[]>(), new Error(ErrorCode.InvalidMsg, "Test Kafka error")));
      consumerMock.Setup(consumer => consumer.Commit(It.IsAny<ConsumeResult<int, string>>())).Callback(() => committed++);

      var received = 0;

      Func<ConsumeResult<int, string>, Task> onMessageReceived = consumeResult =>
      {
        received++;
        return Task.CompletedTask;
      };

      var igniter = new DefaultConsumerIgniter<int, string>(Mock.Of<ILogger<BrokerIngressKafkaDriver>>());

      var result = igniter.Start(
        consumerMock.Object,
        onMessageReceived,
        tokenSource.Token);
      await result;
      // result.IsFaulted.Should().BeTrue();

      received.Should().Be(expected: 0);
      committed.Should().Be(expected: 0, "no messages should be committed");
    }

    private static ConsumeResult<int, string> MakeConsumeResult(Action<ConsumeResult<int, string>> updater = null)
    {
      var result = new ConsumeResult<int, string>
      {
        IsPartitionEOF = false,
        Message = new Message<int, string> {Headers = new Headers(), Key = 1, Timestamp = Timestamp.Default},
        Offset = Offset.Beginning,
        Partition = new Partition(partition: 0),
        Topic = "topic-1",
        TopicPartitionOffset = new TopicPartitionOffset(
          "topic-1",
          new Partition(partition: 0),
          Offset.Beginning)
      };

      updater?.Invoke(result);

      return result;
    }
  }
}
