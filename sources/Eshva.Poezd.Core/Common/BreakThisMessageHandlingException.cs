#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  /// <summary>
  /// Break this message handling signal.
  /// </summary>
  /// <remarks>
  /// This exception used as a signal to stop further message handling and skip following steps in the message handling
  /// pipeline.
  /// </remarks>
  [PublicAPI]
  public class BreakThisMessageHandlingException : PoezdException
  {
    // TODO: Replace this exception with message handling method.
    public BreakThisMessageHandlingException([CanBeNull] string? message) : base(message) { }

    public BreakThisMessageHandlingException([CanBeNull] string? message, Exception innerException) : base(message, innerException) { }
  }
}
