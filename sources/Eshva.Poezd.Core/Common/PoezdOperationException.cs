#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Common
{
  public sealed class PoezdOperationException : PoezdException
  {
    public PoezdOperationException([CanBeNull] string? message) : base(message) { }

    public PoezdOperationException([CanBeNull] string? message, Exception innerException) : base(message, innerException) { }
  }
}
