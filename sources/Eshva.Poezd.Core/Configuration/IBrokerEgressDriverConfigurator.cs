#region Usings

using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public interface IBrokerEgressDriverConfigurator
  {
    void SetDriver(IBrokerEgressDriver driver);
  }
}
