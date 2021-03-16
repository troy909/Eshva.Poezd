#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class BrokerIngressKafkaDriverConfiguration : IMessageRouterConfigurationPart
  {
    public ConsumerConfig ConsumerConfig { get; internal set; }

    public Type HeaderValueCodecType { get; internal set; }

    public Type ConsumerConfiguratorType { get; internal set; }

    public Type DeserializerFactoryType { get; internal set; }

    public Type ConsumerFactoryType { get; internal set; }

    public IEnumerable<string> Validate()
    {
      if (ConsumerConfig == null) yield return "Consumer configuration for broker ingress Kafka driver should be specified.";
      if (HeaderValueCodecType == null) yield return "Header value parser type for broker ingress Kafka driver should be specified.";
      if (ConsumerConfiguratorType == null) yield return "Consumer configurator type for broker ingress Kafka driver should be specified.";
      if (DeserializerFactoryType == null) yield return "Deserializer factory type for broker ingress Kafka driver should be specified.";
      if (ConsumerFactoryType == null) yield return "Consumer factory type for broker ingress Kafka driver should be specified.";
    }
  }
}
