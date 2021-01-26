#region Usings

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Common
{
  public sealed class PoezdOperationException : PoezdException
  {
    public PoezdOperationException([CanBeNull] string? message) : base(message) { }

    public PoezdOperationException([CanBeNull] string? message, Exception innerException) : base(message, innerException) { }

    public PoezdOperationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
