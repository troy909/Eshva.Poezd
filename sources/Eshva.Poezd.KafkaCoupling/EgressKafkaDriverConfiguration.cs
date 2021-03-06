#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class EgressKafkaDriverConfiguration
  {
    public ProducerConfig ProducerConfig { get; internal set; }
  }
}
