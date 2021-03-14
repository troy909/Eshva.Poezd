#region Usings

using Eshva.Poezd.Core.Configuration;
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
  }
}
