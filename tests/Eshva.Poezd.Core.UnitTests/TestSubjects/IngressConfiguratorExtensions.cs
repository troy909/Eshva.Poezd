#region Usings

using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithTestDriver(this BrokerIngressConfigurator ingress, TestDriverState state)
    {
      IBrokerIngressDriverConfigurator driverConfigurator = ingress;
      driverConfigurator.SetDriver(new TestBrokerIngressDriver(state));
      return ingress;
    }
  }
}
