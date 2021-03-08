#region Usings

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  /// <summary>
  /// Break further this message handling signal.
  /// </summary>
  /// <remarks>
  /// This exception used as a signal to stop further message handling and to skip following steps in the message handling
  /// pipeline.
  /// </remarks>
  [PublicAPI]
  public class BreakThisMessageHandlingException : PoezdException
  {
    public BreakThisMessageHandlingException([CanBeNull] string? message) : base(message) { }

    public BreakThisMessageHandlingException([CanBeNull] string? message, Exception innerException) : base(message, innerException) { }

    public BreakThisMessageHandlingException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
