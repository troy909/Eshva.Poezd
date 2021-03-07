#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class BrokerEgressKafkaDriverConfiguration
  {
    public ProducerConfig ProducerConfig { get; internal set; }
  }
}
