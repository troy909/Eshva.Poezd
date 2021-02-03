#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Common;
using FlatSharp;
using FluentAssertions;
using Venture.WorkPlanner.Messages.V1.Events;
using Xunit;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter.UnitTests
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class given_message_type_registry
  {
    [Fact]
    public void when_requested_registered_message_type_it_should_match_its_full_name_to_type_object()
    {
      var sut = new MessageTypeRegistry();
      var expectedType = typeof(TaskCreated);
      sut.GetType(expectedType.FullName!).Should().Be(expectedType, "the message type should be found by its full name");
    }

    [Fact]
    public void when_parsing_registered_message_type_it_should_parse_it_to_object()
    {
      var sut = new MessageTypeRegistry();
      const string expectedTaskType = "some message type";
      var serializedMessage = CreateSerializedMessage(Guid.Empty, expectedTaskType);

      var deserializedMessage = (TaskCreated) sut.Parse(typeof(TaskCreated).FullName!, serializedMessage);

      deserializedMessage.TaskType.Should().Be(expectedTaskType, "message content should be transferred");
    }

    [Fact]
    public void when_parsing_unknown_message_type_it_should_throw()
    {
      var registry = new MessageTypeRegistry();
      var serializedMessage = CreateSerializedMessage(Guid.Empty, string.Empty);
      Action sut = () => registry.Parse("unknown message type", serializedMessage);

      sut.Should().Throw<InvalidOperationException>("a message with unknown type can't be deserialized");
    }

    [Fact]
    public void when_parsing_broken_message_payload_it_should_throw()
    {
      var registry = new MessageTypeRegistry();
      var randomBytes = new byte[10];
      new Random().NextBytes(randomBytes);
      Action sut = () => registry.Parse(typeof(TaskCreated).FullName!, randomBytes);

      sut.Should().Throw<PoezdOperationException>();
    }

    [Fact]
    public void when_parsing_wrong_type_name_it_should_throw()
    {
      var registry = new MessageTypeRegistry();
      var serializedMessage = CreateSerializedMessage(Guid.Empty, string.Empty);

      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => registry.Parse(messageTypeName: null, serializedMessage);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "null is wrong type name");
      sut = () => registry.Parse(string.Empty, serializedMessage);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "an empty string is wrong type name");
      sut = () => registry.Parse(WhitespaceString, serializedMessage);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "a whitespace string is wrong type name");
    }

    [Fact]
    public void when_parsing_with_no_message_payload_specified_it_should_throw()
    {
      var registry = new MessageTypeRegistry();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => registry.Parse(typeof(TaskCreated).FullName!, bytes: null);
      sut.Should().Throw<ArgumentNullException>().Where(exception => exception.ParamName.Equals("bytes"));
    }

    [Fact]
    public void when_requested_not_existing_message_type_it_should_throw()
    {
      Action sut = () => new MessageTypeRegistry().GetType("it is a not existing message type name");
      sut.Should().Throw<InvalidOperationException>("such message type is not exists in the message assembly");
    }

    [Fact]
    public void when_requested_wrong_type_name_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => new MessageTypeRegistry().GetType(messageTypeName: null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "null is wrong type name");
      sut = () => new MessageTypeRegistry().GetType("");
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "an empty string is wrong type name");
      sut = () => new MessageTypeRegistry().GetType(WhitespaceString);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "a whitespace string is wrong type name");
    }

    private static byte[] CreateSerializedMessage(Guid expectedId, string expectedTaskType)
    {
      var task = new TaskCreated {TaskId = expectedId, TaskType = expectedTaskType};
      var serializedMessage = new byte[TaskCreated.Serializer.GetMaxSize(task)];
      TaskCreated.Serializer.Write(serializedMessage, task);
      return serializedMessage;
    }

    private const string WhitespaceString = " \t\n\r";
  }
}
