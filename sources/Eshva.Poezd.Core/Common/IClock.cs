#region Usings

using System;

#endregion

namespace Eshva.Poezd.Core.Common
{
  /// <summary>
  /// The contract of a service of the current time.
  /// </summary>
  public interface IClock
  {
    /// <summary>
    /// Gets the current UTC date/time.
    /// </summary>
    /// <returns>
    /// The current UTC date/time.
    /// </returns>
    DateTimeOffset GetCurrentTimeUtc();
  }
}
