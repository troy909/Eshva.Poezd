#region Usings

using System;
using System.Runtime.Serialization;

#endregion

namespace Eshva.Poezd.Core.Common
{
  public sealed class PoezdConfigurationException : PoezdException
  {
    /// <summary>
    /// Constructs the exception with the given message
    /// </summary>
    public PoezdConfigurationException(string message) : base(message) { }

    /// <summary>
    /// Constructs the exception with the given message and inner exception
    /// </summary>
    public PoezdConfigurationException(string message, Exception innerException) : base(message, innerException) { }

    /// <summary>
    /// Happy cross-domain serialization!
    /// </summary>
    public PoezdConfigurationException(SerializationInfo info, StreamingContext context) : base(info, context) { }
  }
}
