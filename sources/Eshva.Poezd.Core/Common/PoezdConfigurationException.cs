#region Usings

#endregion

namespace Eshva.Poezd.Core.Common
{
  public sealed class PoezdConfigurationException : PoezdException
  {
    /// <summary>
    /// Constructs the exception with the given message
    /// </summary>
    public PoezdConfigurationException(string message) : base(message) { }
  }
}
