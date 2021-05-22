#region Usings

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Base class for configuration objects containing other configuration objects.
  /// </summary>
  public abstract class CompositeMessageRouterConfigurationPart : IMessageRouterConfigurationPart
  {
    /// <inheritdoc />
    public IEnumerable<string> Validate()
    {
      var selfErrors = ValidateItself().ToList();
      var children = GetChildConfigurations().ToArray();
      if (children.Any(part => part == null)) selfErrors.Add($"Some of child properties of {GetType().Name} are null.");

      return selfErrors.Concat(children.Where(part => part != null).SelectMany(part => part.Validate()));
    }

    /// <summary>
    /// Validates the configuration object itself.
    /// </summary>
    /// <returns>
    /// Returns a list of found errors.
    /// </returns>
    [NotNull]
    protected abstract IEnumerable<string> ValidateItself();

    /// <summary>
    /// Gets child configuration objects.
    /// </summary>
    /// <returns>
    /// Returns a list of found errors.
    /// </returns>
    [NotNull]
    protected abstract IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations();
  }
}
