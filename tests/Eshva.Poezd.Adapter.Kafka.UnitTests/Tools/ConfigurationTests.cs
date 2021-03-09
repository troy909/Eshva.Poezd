#region Usings

using System;
using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests.Tools
{
  public static class ConfigurationTests
  {
    public static BrokerEgressKafkaDriverConfiguration CreateBrokerEgressKafkaDriverConfiguration() =>
      new BrokerEgressKafkaDriverConfiguration {ProducerConfig = new ProducerConfig()};

    public static BrokerEgressKafkaDriverConfiguration CreateBrokerEgressKafkaDriverConfigurationWithout(
      Action<BrokerEgressKafkaDriverConfiguration> updater)
    {
      var configuration = CreateBrokerEgressKafkaDriverConfiguration();
      updater(configuration);
      return configuration;
    }
  }
}
