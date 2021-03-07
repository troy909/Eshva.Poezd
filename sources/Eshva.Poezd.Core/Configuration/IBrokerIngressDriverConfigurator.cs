#region Usings

using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public interface IBrokerIngressDriverConfigurator
  {
    void SetDriver(IBrokerIngressDriver driver);
  }
}
