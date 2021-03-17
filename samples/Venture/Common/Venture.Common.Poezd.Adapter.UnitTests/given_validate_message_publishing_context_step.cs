#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Moq;
using Venture.Common.Poezd.Adapter.Egress;
using Xunit;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests
{
  public class given_validate_message_publishing_context_step
  {
    [Fact]
    public void when_executing_without_context_it_should_fail()
    {
      var step = new ValidateMessagePublishingContextStep();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Func<Task> sut = () => step.Execute(context: null);

      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("context"), "context is required");
    }

    [Fact]
    public void when_executing_without_message_in_context_it_should_fail()
    {
      var step = new ValidateMessagePublishingContextStep();
      var context = new MessagePublishingContext
      {
        Api = CreateEgressApi(),
        Message = null,
        CorrelationId = ExpectedCorrelationId,
        CausationId = ExpectedCausationId,
        MessageId = ExpectedMessageId
      };

      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<ArgumentException>()
        .Where(
          exception => exception.ParamName.Equals("context") && exception.Message.Contains("Message should"),
          "message is required");
    }

    [Fact]
    public void when_executing_without_correlation_id_in_context_it_should_fail()
    {
      var step = new ValidateMessagePublishingContextStep();
      var context = new MessagePublishingContext
      {
        Api = CreateEgressApi(),
        Message = new Message1(),
        CorrelationId = ExpectedCorrelationId,
        CausationId = ExpectedCausationId,
        MessageId = ExpectedMessageId
      };

      Func<Task> sut = () => step.Execute(context);

      context.CorrelationId = null;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Correlation ID should"),
        "null is not a valid correlation ID");
      context.CorrelationId = string.Empty;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Correlation ID should"),
        "an empty string is not a valid correlation ID");
      context.CorrelationId = WhitespaceString;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Correlation ID should"),
        "a whitespace string is not a valid correlation ID");
    }

    [Fact]
    public void when_executing_without_causation_id_in_context_it_should_fail()
    {
      var step = new ValidateMessagePublishingContextStep();
      var context = new MessagePublishingContext
      {
        Api = CreateEgressApi(),
        Message = new Message1(),
        CorrelationId = ExpectedCorrelationId,
        CausationId = ExpectedCausationId,
        MessageId = ExpectedMessageId
      };

      Func<Task> sut = () => step.Execute(context);

      context.CausationId = null;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Causation ID should"),
        "null is not a valid causation ID");
      context.CausationId = string.Empty;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Causation ID should"),
        "an empty string is not a valid causation ID");
      context.CausationId = WhitespaceString;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Causation ID should"),
        "a whitespace string is not a valid causation ID");
    }

    [Fact]
    public void when_executing_without_message_id_in_context_it_should_fail()
    {
      var step = new ValidateMessagePublishingContextStep();
      var context = new MessagePublishingContext
      {
        Api = CreateEgressApi(),
        Message = new Message1(),
        CorrelationId = ExpectedCorrelationId,
        CausationId = ExpectedCausationId,
        MessageId = ExpectedMessageId
      };

      Func<Task> sut = () => step.Execute(context);

      context.MessageId = null;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Message ID should"),
        "null is not a valid message ID");
      context.MessageId = string.Empty;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Message ID should"),
        "an empty string is not a valid message ID");
      context.MessageId = WhitespaceString;
      sut.Should().Throw<ArgumentException>().Where(
        exception => exception.ParamName.Equals("context") && exception.Message.Contains("Message ID should"),
        "a whitespace string is not a valid message ID");
    }

    [Fact]
    public void when_executing_without_api_in_context_it_should_fail()
    {
      var step = new ValidateMessagePublishingContextStep();
      var context = new MessagePublishingContext
      {
        Api = null,
        Message = new Message1(),
        CorrelationId = ExpectedCorrelationId,
        CausationId = ExpectedCausationId,
        MessageId = ExpectedMessageId
      };

      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<ArgumentException>()
        .Where(
          exception => exception.ParamName.Equals("context") && exception.Message.Contains("Egress API should"),
          "egress API is required");
    }

    [Fact]
    public void when_executing_without_message_types_registry_in_context_it_should_fail()
    {
      var step = new ValidateMessagePublishingContextStep();
      var context = new MessagePublishingContext
      {
        Api = CreateEgressApi(shouldSetRegistry: false),
        Message = new Message1(),
        CorrelationId = ExpectedCorrelationId,
        CausationId = ExpectedCausationId,
        MessageId = ExpectedMessageId
      };

      Func<Task> sut = () => step.Execute(context);

      sut.Should().Throw<ArgumentException>()
        .Where(
          exception => exception.ParamName.Equals("context") && exception.Message.Contains("Message types registry"),
          "message is required");
    }

    private static IEgressApi CreateEgressApi(bool shouldSetRegistry = true)
    {
      var registryMock = new Mock<IEgressMessageTypesRegistry>();
      registryMock.Setup(registry => registry.GetMessageTypeNameByItsMessageType(It.IsAny<Type>())).Returns(ExpectedMessageTypeName);
      var egressApiMock = new Mock<IEgressApi>();
      egressApiMock.SetupGet(api => api.MessageTypesRegistry).Returns(() => shouldSetRegistry ? registryMock.Object : null);
      return egressApiMock.Object;
    }

    private const string WhitespaceString = " \t\n";
    private const string ExpectedMessageTypeName = nameof(ExpectedMessageTypeName);
    private const string ExpectedCorrelationId = nameof(ExpectedCorrelationId);
    private const string ExpectedCausationId = nameof(ExpectedCausationId);
    private const string ExpectedMessageId = nameof(ExpectedMessageId);

    // ReSharper disable once MemberCanBePrivate.Global
    public class Message1 { }
  }
}
