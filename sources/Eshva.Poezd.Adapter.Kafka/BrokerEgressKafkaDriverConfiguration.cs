#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class BrokerEgressKafkaDriverConfiguration
  {
    public ProducerConfig ProducerConfig { get; internal set; }
  }
}
