#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.Ingress;

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
        ProducerFactoryType = typeof(object),
        HeaderValueCodecType = typeof(object),
        ProducerConfiguratorType = typeof(object),
        SerializerFactoryType = typeof(object)
      };

    public static BrokerEgressKafkaDriverConfiguration CreateBrokerEgressKafkaDriverConfigurationWithout(
      Action<BrokerEgressKafkaDriverConfiguration> updater)
    {
      var configuration = CreateBrokerEgressKafkaDriverConfiguration();
      updater(configuration);
      return configuration;
    }

    public static BrokerIngressKafkaDriverConfiguration CreateBrokerIngressKafkaDriverConfiguration() =>
      new BrokerIngressKafkaDriverConfiguration
      {
        ConsumerConfig = new ConsumerConfig(),
        ConsumerConfiguratorType = typeof(object),
        ConsumerFactoryType = typeof(object),
        DeserializerFactoryType = typeof(object),
        HeaderValueCodecType = typeof(object)
      };

    public static BrokerIngressKafkaDriverConfiguration CreateBrokerIngressKafkaDriverConfigurationWithout(
      Action<BrokerIngressKafkaDriverConfiguration> updater)
    {
      var configuration = CreateBrokerIngressKafkaDriverConfiguration();
      updater(configuration);
      return configuration;
    }
  }
}
