#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public static class EgressConfiguratorExtensions
  {
    public static BrokerEgressConfigurator WithKafkaDriver(
      this BrokerEgressConfigurator brokerEgress,
      Action<BrokerEgressKafkaDriverConfigurator> configurator)
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      configurator(new BrokerEgressKafkaDriverConfigurator(configuration));
      IBrokerEgressDriverConfigurator driverConfigurator = brokerEgress;
      driverConfigurator.SetDriver(new BrokerEgressKafkaDriver(configuration));
      return brokerEgress;
    }
  }
}
