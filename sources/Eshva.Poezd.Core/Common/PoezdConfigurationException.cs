#region Usings

using System;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Eshva.Poezd.Core.Common
{
  [ExcludeFromCodeCoverage]
  public sealed class PoezdConfigurationException : PoezdException
  {
    /// <summary>
    /// Constructs a new instance of Poezd configuration exception.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    public PoezdConfigurationException(string message) : base(message) { }

    /// <summary>
    /// Constructs a new instance of Poezd configuration exception.
    /// </summary>
    /// <param name="message">
    /// The message.
    /// </param>
    /// <param name="innerException">
    /// The cause of this exception.
    /// </param>
    public PoezdConfigurationException(string message, Exception innerException) : base(message, innerException) { }
  }
}
