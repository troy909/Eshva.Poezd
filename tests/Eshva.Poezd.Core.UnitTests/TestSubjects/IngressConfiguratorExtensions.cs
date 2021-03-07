#region Usings

using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithTestDriver(this BrokerIngressConfigurator ingress)
    {
      IBrokerIngressDriverConfigurator driverConfigurator = ingress;
      driverConfigurator.SetDriver(new TestBrokerIngressDriver());
      return ingress;
    }
  }
}
