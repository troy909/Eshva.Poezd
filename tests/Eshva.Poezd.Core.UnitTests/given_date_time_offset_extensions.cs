#region Usings

using System;
using Eshva.Poezd.Core.Common;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_date_time_offset_extensions
  {
    [Fact]
    public void when_check_is_date_time_missing_for_missing_value_it_should_return_true()
    {
      DateTimeOffset.MinValue.IsMissing().Should().BeTrue("DateTimeOffset.MinValue is a missing value");
    }

    [Fact]
    public void when_check_is_date_time_missing_for_not_missed_value_it_should_return_false()
    {
      DateTimeOffset.UtcNow.IsMissing().Should().BeFalse("current DateTimeOffset is not a missing value");
    }
  }
}
