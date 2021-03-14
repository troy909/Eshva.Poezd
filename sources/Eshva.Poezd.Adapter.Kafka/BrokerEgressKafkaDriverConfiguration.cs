#region Usings

using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class BrokerEgressKafkaDriverConfiguration : IMessageRouterConfigurationPart

  {
    public ProducerConfig ProducerConfig { get; internal set; }

    public IProducerFactory ProducerFactory { get; internal set; }

    public IEnumerable<string> Validate()
    {
      if (ProducerConfig == null)
        yield return "Producer configuration for broker egress Kafka driver should be specified.";
      if (ProducerFactory == null)
        yield return "Producer factory for broker egress Kafka driver should be specified.";
    }
  }
}
