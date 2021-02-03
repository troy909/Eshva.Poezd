#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using FluentAssertions;
using Venture.WorkPlanner.Messages.V1.Events;
using Xunit;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter.UnitTests
{
  [SuppressMessage("ReSharper", "InconsistentNaming")]
  public class given_message_type_matcher
  {
    [Fact]
    public void when_requested_existing_message_type_it_should_match_its_full_name_to_type_object()
    {
      var sut = new MessageTypeMatcher();
      var expectedType = typeof(TaskCreated);
      sut.MatchToType(expectedType.FullName!).Should().Be(expectedType, "the message type should be found by its full name");
    }

    [Fact]
    public void when_requested_not_existing_message_type_it_should_throw()
    {
      Action sut = () => new MessageTypeMatcher().MatchToType("it is a not existing message type name");
      sut.Should().Throw<InvalidOperationException>("such message type is not exists in the message assembly");
    }

    [Fact]
    public void when_requested_wrong_type_name_it_should_throw()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test.
      Action sut = () => new MessageTypeMatcher().MatchToType(messageTypeName: null);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "null is wrong type name");
      sut = () => new MessageTypeMatcher().MatchToType("");
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "an empty string is wrong type name");
      sut = () => new MessageTypeMatcher().MatchToType(WhitespaceString);
      sut.Should().Throw<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("messageTypeName"),
        "a whitespace string is wrong type name");
    }

    private const string WhitespaceString = " \t\n\r";
  }
}
