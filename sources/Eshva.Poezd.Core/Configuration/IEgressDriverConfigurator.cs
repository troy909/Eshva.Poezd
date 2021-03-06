#region Usings

using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public interface IEgressDriverConfigurator
  {
    void SetDriver(IEgressDriver driver);
  }
}
