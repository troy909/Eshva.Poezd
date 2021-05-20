#region Usings

using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Routing;
using Moq;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithTestDriver(this BrokerIngressConfigurator ingress, TestDriverState state)
    {
      IBrokerIngressDriverConfigurator driverConfigurator = ingress;
      driverConfigurator.SetDriver(new TestBrokerIngressDriver(state), Mock.Of<IMessageRouterConfigurationPart>());
      return ingress;
    }

    public static BrokerIngressConfigurator WithSpecificDriver(this BrokerIngressConfigurator ingress, IBrokerIngressDriver driver)
    {
      IBrokerIngressDriverConfigurator driverConfigurator = ingress;
      driverConfigurator.SetDriver(driver, Mock.Of<IMessageRouterConfigurationPart>());
      return ingress;
    }
  }
}
