#region Usings

using System;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestClock : IClock
  {
    public TestClock(DateTimeOffset time)
    {
      _time = time;
    }

    public DateTimeOffset GetCurrentTimeUtc() => _time;

    private readonly DateTimeOffset _time;
  }
}
