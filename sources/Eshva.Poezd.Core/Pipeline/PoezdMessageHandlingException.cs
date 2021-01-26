#region Usings

using System;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public sealed class PoezdMessageHandlingException : Exception
  {
    public PoezdMessageHandlingException(string message) : base(message) { }
  }
}
