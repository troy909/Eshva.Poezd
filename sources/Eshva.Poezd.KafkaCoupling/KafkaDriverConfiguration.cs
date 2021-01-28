#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class KafkaDriverConfiguration
  {
    public ConsumerConfig ConsumerConfig { get; set; }

    public ProducerConfig ProducerConfig { get; set; }

    public long CommitPeriod { get; set; }

    public string GroupId { get; set; }
  }
}
