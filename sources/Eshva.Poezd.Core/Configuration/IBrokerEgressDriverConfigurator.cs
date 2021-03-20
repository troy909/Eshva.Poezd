#region Usings

using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// The contract of the broker egress driver configurator.
  /// </summary>
  public interface IBrokerEgressDriverConfigurator
  {
    /// <summary>
    /// Sets the driver and its configuration.
    /// </summary>
    /// <param name="driver">
    /// The driver to set.
    /// </param>
    /// <param name="configuration">
    /// The driver configuration to set.
    /// </param>
    /// <exception cref="System.ArgumentNullException">
    /// Some argument is not specified.
    /// </exception>
    void SetDriver([NotNull] IBrokerEgressDriver driver, [NotNull] IMessageRouterConfigurationPart configuration);
  }
}
