#region Usings

using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public interface IBrokerIngressDriverConfigurator
  {
    void SetDriver(
      [NotNull] IBrokerIngressDriver driver,
      [NotNull] IMessageRouterConfigurationPart configuration);
  }
}
