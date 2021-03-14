#region Usings

using Eshva.Poezd.Core.Configuration;
using Moq;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public static class EgressConfiguratorExtensions
  {
    public static BrokerEgressConfigurator WithTestDriver(this BrokerEgressConfigurator egress, TestDriverState state)
    {
      IBrokerEgressDriverConfigurator driverConfigurator = egress;
      driverConfigurator.SetDriver(new TestBrokerEgressDriver(state), Mock.Of<IMessageRouterConfigurationPart>());
      return egress;
    }
  }
}
