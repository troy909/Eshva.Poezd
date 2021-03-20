#region Usings

using System;

#endregion

namespace Eshva.Poezd.Core.Common
{
  /// <summary>
  /// Additional methods for <see cref="DateTimeOffset" />.
  /// </summary>
  public static class DateTimeOffsetExtensions
  {
    /// <summary>
    /// Checks is this <see cref="DateTimeOffset" /> variable is not specified. It's like <c>null</c> checking for reference
    /// types.
    /// </summary>
    /// <remarks>
    /// Missing value is <see cref="DateTimeOffset.MinValue" />.
    /// </remarks>
    /// <param name="value">
    /// Value to check.
    /// </param>
    /// <returns>
    /// <c>true</c> - the value of a variable is missing, <c>false</c> - the value is specified.
    /// </returns>
    public static bool IsMissing(this DateTimeOffset value) => value.Equals(DateTimeOffset.MinValue);
  }
}
