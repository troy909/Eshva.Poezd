#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using RandomStringCreator;
using Venture.Common.Application.Egress;
using Xunit;

#endregion

namespace Venture.Common.Application.UnitTests
{
  public class given_venture_outgoing_message_handling_context
  {
    [Fact]
    public void when_created_with_all_arguments_specified_it_should_initialize_instance_with_their_values()
    {
      var message = new object();
      var generatedOnUtc = DateTimeOffset.UtcNow;
      var correlationId = _stringCreator.Get(length: 10);
      var causationId = _stringCreator.Get(length: 10);
      var messageId = _stringCreator.Get(length: 10);
      var context = new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        correlationId,
        causationId,
        messageId);

      context.Message.Should().BeSameAs(message);
      context.CorrelationId.Should().BeSameAs(correlationId);
      context.CausationId.Should().BeSameAs(causationId);
      context.MessageId.Should().BeSameAs(messageId);
      context.ProducedOnUtc.Should().Be(generatedOnUtc);
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_created_with_some_attribute_not_specified_it_should_fail()
    {
      var message = new object();
      var generatedOnUtc = DateTimeOffset.UtcNow;
      var correlationId = _stringCreator.Get(length: 10);
      var causationId = _stringCreator.Get(length: 10);
      var messageId = _stringCreator.Get(length: 10);
      Action sut = () => new VentureOutgoingMessageHandlingContext(
        message: null,
        generatedOnUtc,
        correlationId,
        causationId,
        messageId);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("message"));

      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        correlationId: null,
        causationId,
        messageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("correlationId"),
        "null is not valid correlation ID");
      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        string.Empty,
        causationId,
        messageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("correlationId"),
        "an empty string is not valid correlation ID");
      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        WhitespaceString,
        causationId,
        messageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("correlationId"),
        "a whitespace string is not valid correlation ID");

      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        correlationId,
        causationId: null,
        messageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("causationId"),
        "null is not valid causation ID");
      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        correlationId,
        string.Empty,
        messageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("causationId"),
        "an empty string is not valid causation ID");
      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        correlationId,
        WhitespaceString,
        messageId);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("causationId"),
        "a whitespace string is not valid causation ID");

      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        correlationId,
        causationId,
        messageId: null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageId"),
        "null is not valid message ID");
      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        correlationId,
        causationId,
        string.Empty);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageId"),
        "an empty string is not valid message ID");
      sut = () => new VentureOutgoingMessageHandlingContext(
        message,
        generatedOnUtc,
        correlationId,
        causationId,
        WhitespaceString);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageId"),
        "a whitespace string is not valid message ID");
    }

    private readonly StringCreator _stringCreator = new StringCreator();
    private const string WhitespaceString = " \t\n";
  }
}
