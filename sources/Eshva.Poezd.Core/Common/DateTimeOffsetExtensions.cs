#region Usings

using System;

#endregion

namespace Eshva.Poezd.Core.Common
{
  public static class DateTimeOffsetExtensions
  {
    public static bool IsMissing(this DateTimeOffset value) => value.Equals(DateTimeOffset.MinValue);
  }
}
