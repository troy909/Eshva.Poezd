#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using FluentAssertions;
using RandomStringCreator;
using Venture.Common.Application.Egress;
using Venture.Common.Poezd.Adapter.Egress;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_message_publisher_extensions
  {
    [Fact]
    public async Task when_publishing_inception_message_it_should_assign_proper_message_ids()
    {
      var messageRouter = new FakeMessageRouter();
      var sut = new VentureMessagePublisher(messageRouter);
      var message = new object();

      await sut.PublishInceptionMessage(message, DateTimeOffset.UtcNow);

      messageRouter.Message.Should().BeSameAs(message, "it should pass message without changes");
      messageRouter.MessageId.Should().NotBeNullOrWhiteSpace("it should generate message ID");
      messageRouter.CorrelationId.Should().Be(messageRouter.MessageId, "correlation ID and message ID should be the same");
      messageRouter.CausationId.Should().Be(messageRouter.MessageId, "causation ID and message ID should be the same");
    }

    [Fact]
    public void when_publishing_null_as_inception_message_it_should_fail()
    {
      var publisher = new VentureMessagePublisher(new FakeMessageRouter());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => publisher.PublishInceptionMessage<object>(message: null, DateTimeOffset.UtcNow);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("message"), "message should be specified");
    }

    [Fact]
    public async Task when_publishing_subsequent_message_it_should_assign_proper_message_ids()
    {
      var messageRouter = new FakeMessageRouter();
      var sut = new VentureMessagePublisher(messageRouter);
      var message = new object();

      var correlationId = _stringCreator.Get(length: 10);
      var precedingMessageId = _stringCreator.Get(length: 10);
      await sut.PublishSubsequentMessage(
        message,
        DateTimeOffset.UtcNow,
        correlationId,
        precedingMessageId);

      messageRouter.Message.Should().BeSameAs(message, "it should pass message without changes");
      messageRouter.MessageId.Should().NotBeNullOrWhiteSpace("it should generate message ID");
      messageRouter.CorrelationId.Should().Be(correlationId, "it should pass correlation ID without changes");
      messageRouter.CausationId.Should().Be(precedingMessageId, "it should use preceding message ID as causation ID");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_publishing_subsequent_message_with_some_attribute_not_specified_it_should_fail()
    {
      var messageRouter = new FakeMessageRouter();
      var publisher = new VentureMessagePublisher(messageRouter);
      var message = new object();

      var correlationId = _stringCreator.Get(length: 10);
      var precedingMessageId = _stringCreator.Get(length: 10);
      var generatedOnUtc = DateTimeOffset.UtcNow;

      Func<Task> sut = () => publisher.PublishSubsequentMessage<object>(
        message: null,
        generatedOnUtc,
        correlationId,
        precedingMessageId);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("message"));

      sut = () => publisher.PublishSubsequentMessage(
        message,
        generatedOnUtc,
        correlationId: null,
        precedingMessageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("correlationId"),
        "null is not valid correlation ID");
      sut = () => publisher.PublishSubsequentMessage(
        message,
        generatedOnUtc,
        string.Empty,
        precedingMessageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("correlationId"),
        "an empty string is not valid correlation ID");
      sut = () => publisher.PublishSubsequentMessage(
        message,
        generatedOnUtc,
        WhitespaceString,
        precedingMessageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("correlationId"),
        "a whitespace string is not valid correlation ID");

      sut = () => publisher.PublishSubsequentMessage(
        message,
        generatedOnUtc,
        correlationId,
        precedingMessageId: null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("precedingMessageId"),
        "null is not valid preceding message ID");
      sut = () => publisher.PublishSubsequentMessage(
        message,
        generatedOnUtc,
        correlationId,
        string.Empty);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("precedingMessageId"),
        "an empty string is not valid preceding message ID");
      sut = () => publisher.PublishSubsequentMessage(
        message,
        generatedOnUtc,
        correlationId,
        WhitespaceString);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("precedingMessageId"),
        "a whitespace string is not valid preceding message ID");
    }

    private readonly StringCreator _stringCreator = new StringCreator();
    private const string WhitespaceString = " \t\n";
  }
}
