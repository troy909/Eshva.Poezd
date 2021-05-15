#region Usings

using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Routing;
using Moq;

#endregion

namespace Eshva.Poezd.Core.IntegrationTests.Tools
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithTestDriver(this BrokerIngressConfigurator ingress)
    {
      IBrokerIngressDriverConfigurator driverConfigurator = ingress;
      driverConfigurator.SetDriver(Mock.Of<IBrokerIngressDriver>(), Mock.Of<IMessageRouterConfigurationPart>());
      return ingress;
    }
  }
}
