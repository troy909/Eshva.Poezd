#region Usings

using System;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Xunit;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class given_wildcard_queue_name_matcher
  {
    [Fact]
    public void when_matching_of_equal_queue_name_and_pattern_requested_it_should_return_true()
    {
      var sut = new RegexQueueNameMatcher();
      const string QueueName = "queue-name";
      sut.IsMatch(QueueName, QueueName).Should().BeTrue("queue name and pattern are the same");
    }

    [Fact]
    public void when_matching_of_different_queue_name_and_pattern_requested_it_should_return_false()
    {
      var sut = new RegexQueueNameMatcher();
      sut.IsMatch("queue-name", "queue-name-pattern").Should().BeFalse("queue name and pattern are the different");
    }

    [Fact]
    public void when_matching_of_queue_name_and_pattern_different_only_with_case_requested_it_should_return_false()
    {
      var sut = new RegexQueueNameMatcher();
      const string QueueName = "queue-name";
      sut.IsMatch(QueueName, QueueName.ToUpper()).Should().BeFalse("case of queue name and pattern are the different");
    }

    [Fact]
    public void when_matching_of_queue_name_matching_pattern_requested_it_should_return_true()
    {
      var sut = new RegexQueueNameMatcher();
      const string QueueName1 = "queue-name";
      sut.IsMatch(QueueName1, QueueName1 + ".*").Should().BeTrue("queue name matching the pattern");
      sut.IsMatch(QueueName1 + ".some-suffix", QueueName1 + "*").Should().BeTrue("queue name matching the pattern");
      sut.IsMatch("sample.facts.case.v1", "sample.(facts|commands).case.v1").Should().BeTrue("queue name matching the pattern");
    }

    [Fact]
    public void when_matching_queue_name_is_null_empty_or_whitespace_it_should_throw()
    {
      var matcher = new RegexQueueNameMatcher();

      // ReSharper disable once AssignNullToNotNullAttribute
      new Action(() => matcher.IsMatch(null, "something"))
        .Should().Throw<ArgumentNullException>("null is illegal as queue name");
      new Action(() => matcher.IsMatch(string.Empty, "something"))
        .Should().Throw<ArgumentNullException>("empty string is illegal as queue name");
      new Action(() => matcher.IsMatch("    \n", "something"))
        .Should().Throw<ArgumentNullException>("empty string is illegal as queue name");
    }

    [Fact]
    public void when_matching_queue_name_pattern_is_null_empty_or_whitespace_it_should_throw()
    {
      var matcher = new RegexQueueNameMatcher();

      // ReSharper disable once AssignNullToNotNullAttribute
      new Action(() => matcher.IsMatch("something", null))
        .Should().Throw<ArgumentNullException>("null is illegal as queue name pattern");
      new Action(() => matcher.IsMatch("something", string.Empty))
        .Should().Throw<ArgumentNullException>("empty string is illegal as queue name pattern");
      new Action(() => matcher.IsMatch("something", "    \n"))
        .Should().Throw<ArgumentNullException>("empty string is illegal as queue name pattern");
    }
  }
}
