#region Usings

using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public static class EgressConfiguratorExtensions
  {
    public static BrokerEgressConfigurator WithTestDriver(this BrokerEgressConfigurator egress, TestDriverState state)
    {
      IBrokerEgressDriverConfigurator driverConfigurator = egress;
      driverConfigurator.SetDriver(new TestBrokerEgressDriver(state));
      return egress;
    }
  }
}
