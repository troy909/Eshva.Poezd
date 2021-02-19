#region Usings

using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Message router configuration part.
  /// </summary>
  public interface IMessageRouterConfigurationPart
  {
    /// <summary>
    /// Validates this configuration object.
    /// </summary>
    /// <returns>
    /// Returns a list of found errors.
    /// </returns>
    [NotNull]
    IEnumerable<string> Validate();
  }
}
