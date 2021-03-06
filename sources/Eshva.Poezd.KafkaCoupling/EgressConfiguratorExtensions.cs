using System;
using Eshva.Poezd.Core.Configuration;

namespace Eshva.Poezd.KafkaCoupling
{
  public static class EgressConfiguratorExtensions
  {
    public static EgressConfigurator WithKafkaDriver(this EgressConfigurator egress, Action<EgressKafkaDriverConfigurator> configurator)
    {
      var configuration = new EgressKafkaDriverConfiguration();
      configurator(new EgressKafkaDriverConfigurator(configuration));
      IEgressDriverConfigurator driverConfigurator = egress;
      driverConfigurator.SetDriver(new EgressKafkaDriver(configuration));
      return egress;
    }
  }
}
