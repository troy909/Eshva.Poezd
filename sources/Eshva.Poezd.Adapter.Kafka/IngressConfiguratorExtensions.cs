#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithKafkaDriver(
      this BrokerIngressConfigurator ingress,
      Action<BrokerIngressKafkaDriverConfigurator> configurator)
    {
      var configuration = new BrokerIngressKafkaDriverConfiguration();
      configurator(new BrokerIngressKafkaDriverConfigurator(configuration));
      IBrokerIngressDriverConfigurator driverConfigurator = ingress;
      driverConfigurator.SetDriver(new BrokerIngressKafkaDriver(configuration));
      return ingress;
    }
  }
}
