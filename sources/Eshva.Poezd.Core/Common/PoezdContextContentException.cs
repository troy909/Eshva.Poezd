#region Usings

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  public class PoezdContextContentException : PoezdException
  {
    public PoezdContextContentException([CanBeNull] string? message, string contextItemName) : base(message)
    {
      ContextItemName = contextItemName;
    }

    public PoezdContextContentException(
      [CanBeNull] string? message,
      string contextItemName,
      Exception innerException) : base(message, innerException)
    {
      ContextItemName = contextItemName;
    }

    public PoezdContextContentException(SerializationInfo info, StreamingContext context) : base(info, context) { }

    public string ContextItemName { get; }
  }
}
