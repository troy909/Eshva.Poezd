#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class BrokerEgressKafkaDriverConfiguration : IMessageRouterConfigurationPart

  {
    public ProducerConfig ProducerConfig { get; internal set; }

    public Type ProducerFactoryType { get; internal set; }

    public Type ProducerConfiguratorType { get; internal set; }

    public Type SerializerFactoryType { get; internal set; }

    public Type HeaderValueCodecType { get; internal set; }

    public IEnumerable<string> Validate()
    {
      if (ProducerConfig == null)
        yield return "Producer configuration for broker egress Kafka driver should be specified.";
      if (ProducerFactoryType == null)
        yield return "Producer factory type for broker egress Kafka driver should be specified.";
      if (ProducerConfiguratorType == null)
        yield return "Producer configurator type for broker egress Kafka driver should be specified.";
      if (SerializerFactoryType == null)
        yield return "Serializer factory type for broker egress Kafka driver should be specified.";
      if (HeaderValueCodecType == null)
        yield return "Header value codec type for broker egress Kafka driver should be specified.";
    }
  }
}
