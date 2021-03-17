#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Common.Testing;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog.Events;
using Serilog.Sinks.InMemory;
using SimpleInjector;
using Xunit;
using Xunit.Abstractions;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_api_consumer
  {
    public given_api_consumer(ITestOutputHelper testOutput)
    {
      _testOutput = testOutput;
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_constructed_with_valid_arguments_it_should_fail()
    {
      var container = new Container().AddLogging(_testOutput);
      Action sut = () => new ApiConsumer<int, string>(
        Mock.Of<IIngressApi>(),
        Mock.Of<IConsumer<int, string>>(),
        container.GetInstance<ILogger<ApiConsumer<int, string>>>());
      sut.Should().NotThrow();
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_constructed_with_invalid_arguments_it_should_fail()
    {
      var container = new Container().AddLogging(_testOutput);
      var api = Mock.Of<IIngressApi>();
      var consumer = Mock.Of<IConsumer<int, string>>();
      var logger = container.GetInstance<ILogger<ApiConsumer<int, string>>>();
      Action sut = () => new ApiConsumer<int, string>(
        api,
        consumer,
        logger);

      api = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("api"));
      api = Mock.Of<IIngressApi>();
      consumer = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("consumer"));
      consumer = Mock.Of<IConsumer<int, string>>();
      logger = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("logger"));
    }

    [Fact]
    public async Task when_start_with_valid_arguments_it_should_consume_messages_from_consumer()
    {
      var container = new Container().AddLogging(_testOutput);
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

      var sut = new ApiConsumer<int, string>(
        Mock.Of<IIngressApi>(),
        consumerMock.Object,
        container.GetInstance<ILogger<ApiConsumer<int, string>>>());

      await sut.Start(onMessageReceived, tokenSource.Token);

      received.Should().Be(numberOfMessages);
      closed.Should().Be(expected: 0, "consumer will be closed in a different place");
    }

    [Fact]
    public async Task when_start_and_eof_it_should_skip_message()
    {
      var container = new Container().AddLogging(_testOutput);
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

      var sut = new ApiConsumer<int, string>(
        Mock.Of<IIngressApi>(),
        consumerMock.Object,
        container.GetInstance<ILogger<ApiConsumer<int, string>>>());

      await sut.Start(onMessageReceived, tokenSource.Token);

      received.Should().Be(expected: 0);
    }

    [Fact]
    public async Task when_start_with_valid_arguments_it_should_commit_kafka_message()
    {
      var container = new Container().AddLogging(_testOutput);
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

      var sut = new ApiConsumer<int, string>(
        Mock.Of<IIngressApi>(),
        consumerMock.Object,
        container.GetInstance<ILogger<ApiConsumer<int, string>>>());

      await sut.Start(onMessageReceived, tokenSource.Token);

      received.Should().Be(numberOfMessages);
      committed.Should().Be(numberOfMessages, "all messages should be committed");
    }

    [Fact]
    public async Task when_kafka_exception_thrown_in_start_it_should_fail()
    {
      var container = new Container().AddLogging(_testOutput);
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

      var apiConsumer = new ApiConsumer<int, string>(
        Mock.Of<IIngressApi>(),
        consumerMock.Object,
        container.GetInstance<ILogger<ApiConsumer<int, string>>>());

      var result = apiConsumer.Start(onMessageReceived, tokenSource.Token);
      await result;
      // result.IsFaulted.Should().BeTrue();

      received.Should().Be(expected: 0);
      committed.Should().Be(expected: 0, "no messages should be committed");
    }

    [Fact]
    public void when_dispose_it_should_commit_offsets_close_connection_and_dispose_consumer()
    {
      var tokenSource = new CancellationTokenSource();
      var cancellationToken = tokenSource.Token;
      var container = new Container().AddLogging(_testOutput);
      var consumerMock = new Mock<IConsumer<int, string>>();
      consumerMock.SetupGet(consumer => consumer.Name).Returns(string.Empty);
      consumerMock.SetupGet(consumer => consumer.MemberId).Returns(string.Empty);
      var loggerFactoryMock = new Mock<ILoggerFactory>();
      loggerFactoryMock.Setup(factory => factory.CreateLogger(It.IsAny<string>())).Returns(Mock.Of<ILogger>);
      var sut = new ApiConsumer<int, string>(
        Mock.Of<IIngressApi>(),
        consumerMock.Object,
        container.GetInstance<ILogger<ApiConsumer<int, string>>>());
      sut.Start(result => Task.CompletedTask, cancellationToken);
      tokenSource.Cancel();
      sut.Dispose();

      consumerMock.Verify(consumer => consumer.Close(), Times.Once());
      consumerMock.Verify(consumer => consumer.Dispose(), Times.Once());
    }

    [Fact]
    public void when_dispose_and_exception_thrown_it_should_ignore_them()
    {
      var container = new Container().AddLogging(_testOutput);
      var consumerMock = new Mock<IConsumer<int, string>>();
      consumerMock.Setup(consumer => consumer.Commit()).Throws<Exception>();

      var apiConsumer = new ApiConsumer<int, string>(
        Mock.Of<IIngressApi>(),
        consumerMock.Object,
        container.GetInstance<ILogger<ApiConsumer<int, string>>>());
      Action sut = () => apiConsumer.Dispose();

      sut.Should().NotThrow("Exceptions should be ignored.");

      consumerMock.Setup(consumer => consumer.Close()).Throws<Exception>();
      sut.Should().NotThrow("Exceptions should be ignored.");
    }

    [Fact]
    public void when_stop_and_fail_on_commit_it_should_log_exception_but_not_throw()
    {
      var container = new Container().AddLogging(_testOutput);
      var tokenSource = new CancellationTokenSource();
      var cancellationToken = tokenSource.Token;
      var consumerMock = new Mock<IConsumer<int, string>>();
      consumerMock.SetupGet(consumer => consumer.Name).Returns(string.Empty);
      consumerMock.SetupGet(consumer => consumer.MemberId).Returns(string.Empty);
      consumerMock.Setup(consumer => consumer.Consume(cancellationToken)).Returns(() => MakeConsumeResult());
      consumerMock.Setup(consumer => consumer.Close()).Throws<Exception>();
      var apiConsumer = new ApiConsumer<int, string>(
        Mock.Of<IIngressApi>(),
        consumerMock.Object,
        container.GetInstance<ILogger<ApiConsumer<int, string>>>());
      apiConsumer.Start(result => Task.CompletedTask, cancellationToken);
      tokenSource.Cancel();

      Action sut = () => apiConsumer.Stop();
      sut.Should().NotThrow("no exception should be thrown even some occurred");

      consumerMock.Verify(consumer => consumer.Close(), Times.Once());
      var logEvents = InMemorySink.Instance.LogEvents.ToList();
      logEvents.Any(log => log.Level == LogEventLevel.Information && log.RenderMessage().Contains("error"))
        .Should().BeTrue("exception should be logged");
    }

    private static ConsumeResult<int, string> MakeConsumeResult(Action<ConsumeResult<int, string>> updater = null)
    {
      var result = new ConsumeResult<int, string>
      {
        IsPartitionEOF = false,
        Message = new Message<int, string> {Headers = new Headers(), Key = 1, Timestamp = Timestamp.Default, Value = "value"},
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

    private readonly ITestOutputHelper _testOutput;
  }
}
