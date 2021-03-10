#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests.Tools
{
  public static class ConfigurationTests
  {
    public static BrokerEgressKafkaDriverConfiguration CreateBrokerEgressKafkaDriverConfiguration(
      Dictionary<string, object> publishedMessages = default,
      Exception exceptionToThrowOnPublishing = default) =>
      new BrokerEgressKafkaDriverConfiguration
      {
        ProducerConfig = new ProducerConfig(),
        ProducerFactory = new TestProducerFactory(publishedMessages ?? new Dictionary<string, object>(), exceptionToThrowOnPublishing)
      };

    public static BrokerEgressKafkaDriverConfiguration CreateBrokerEgressKafkaDriverConfigurationWithout(
      Action<BrokerEgressKafkaDriverConfiguration> updater)
    {
      var configuration = CreateBrokerEgressKafkaDriverConfiguration();
      updater(configuration);
      return configuration;
    }
  }
}
