#region Usings

using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Routing;
using Moq;

#endregion

namespace Eshva.Poezd.Core.IntegrationTests.Tools
{
  public static class EgressConfiguratorExtensions
  {
    public static BrokerEgressConfigurator WithTestDriver(this BrokerEgressConfigurator egress)
    {
      IBrokerEgressDriverConfigurator driverConfigurator = egress;
      driverConfigurator.SetDriver(Mock.Of<IBrokerEgressDriver>(), Mock.Of<IMessageRouterConfigurationPart>());
      return egress;
    }
  }
}
