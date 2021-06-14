#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common.Testing;
using FluentAssertions;
using Venture.Common.Application.Egress;
using Venture.Common.Poezd.Adapter.Egress;
using Venture.Common.Poezd.Adapter.UnitTests.TestSubjects;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_venture_message_publisher
  {
    [Fact]
    public async Task when_publishing_message_it_should_route_it_using_message_router_with_parameters_unchanged()
    {
      var messageRouter = new FakeMessageRouter();
      var sut = new VentureMessagePublisher(messageRouter);
      var message = new object();
      var correlationId = Randomize.String(length: 10);
      var causationId = Randomize.String(length: 10);
      var messageId = Randomize.String(length: 10);

      await sut.Publish(
        message,
        new VentureOutgoingMessageHandlingContext(
          message,
          DateTimeOffset.UtcNow,
          correlationId,
          causationId,
          messageId));

      messageRouter.Message.Should().BeSameAs(message, "it should pass message without changes");
      messageRouter.CorrelationId.Should().BeSameAs(correlationId, "it should pass correlation ID without changes");
      messageRouter.CausationId.Should().BeSameAs(causationId, "it should pass causation ID without changes");
      messageRouter.MessageId.Should().BeSameAs(messageId, "it should pass message ID without changes");
    }

    [Fact]
    public async Task when_message_id_is_not_specified_it_should_generate_it()
    {
      var messageRouter = new FakeMessageRouter();
      var sut = new VentureMessagePublisher(messageRouter);
      var message = new object();
      var correlationId = Randomize.String(length: 10);
      var causationId = Randomize.String(length: 10);
      var messageId = Randomize.String(length: 10);

      await sut.Publish(
        message,
        new VentureOutgoingMessageHandlingContext(
          message,
          DateTimeOffset.UtcNow,
          correlationId,
          causationId,
          messageId));

      messageRouter.Message.Should().BeSameAs(message, "it should pass message without changes");
      messageRouter.CorrelationId.Should().BeSameAs(correlationId, "it should pass correlation ID without changes");
      messageRouter.CausationId.Should().BeSameAs(causationId, "it should pass causation ID without changes");
      messageRouter.MessageId.Should().NotBeNullOrWhiteSpace("message ID should be generated");
    }

    [Fact]
    public void when_publishing_null_as_message_it_should_fail()
    {
      var publisher = new VentureMessagePublisher(new FakeMessageRouter());
      Func<Task> sut = () => publisher.Publish<object>(
        // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
        message: null,
        new VentureOutgoingMessageHandlingContext(
          new object(),
          DateTimeOffset.UtcNow,
          Randomize.String(length: 10),
          Randomize.String(length: 10),
          Randomize.String(length: 10)));

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("message"), "message is required");
    }

    [Fact]
    public void when_publishing_message_without_context_it_should_fail()
    {
      var publisher = new VentureMessagePublisher(new FakeMessageRouter());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => publisher.Publish(new object(), context: null);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"), "context is required");
    }
  }
}
