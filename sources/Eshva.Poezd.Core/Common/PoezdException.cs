#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace Eshva.Poezd.Core.Common
{
  public abstract class PoezdException : ApplicationException
  {
    protected PoezdException(string? message) : base(message) { }

    protected PoezdException(string? message, Exception innerException) : base(message, innerException) { }

    protected PoezdException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
