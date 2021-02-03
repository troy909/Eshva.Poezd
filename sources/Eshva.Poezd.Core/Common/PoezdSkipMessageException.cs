#region Usings

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  public class PoezdSkipMessageException : PoezdException
  {
    public PoezdSkipMessageException([CanBeNull] string? message) : base(message) { }

    public PoezdSkipMessageException([CanBeNull] string? message, Exception innerException) : base(message, innerException) { }

    public PoezdSkipMessageException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
