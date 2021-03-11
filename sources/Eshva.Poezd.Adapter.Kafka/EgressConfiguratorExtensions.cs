#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
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
      driverConfigurator.SetDriver(
        new BrokerEgressKafkaDriver(configuration, new CachingProducerRegistry(configuration.ProducerFactory)),
        configuration);
      return brokerEgress;
    }
  }
}
