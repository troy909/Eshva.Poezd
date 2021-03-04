using Confluent.Kafka;

namespace Eshva.Poezd.KafkaCoupling
{
  public class EgressKafkaDriverConfiguration
  {
    public ProducerConfig ProducerConfig { get; set; }
  }
}