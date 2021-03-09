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

    // TODO: Connect validation to the router configuration validation.
    public IEnumerable<string> Validate()
    {
      if (ProducerConfig == null)
        yield return "Producer configuration for broker egress driver should be specified.";
    }
  }
}
