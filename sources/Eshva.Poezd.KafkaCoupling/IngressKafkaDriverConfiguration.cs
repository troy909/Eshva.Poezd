using System;
using Confluent.Kafka;

namespace Eshva.Poezd.KafkaCoupling
{
  public class IngressKafkaDriverConfiguration
  {
    public ConsumerConfig ConsumerConfig { get; set; }

    public Type HeaderValueParserType { get; set; }
  }
}