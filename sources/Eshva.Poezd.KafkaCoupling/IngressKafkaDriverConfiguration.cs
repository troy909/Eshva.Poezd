#region Usings

using System;
using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class IngressKafkaDriverConfiguration
  {
    public ConsumerConfig ConsumerConfig { get; internal set; }

    public Type HeaderValueParserType { get; internal set; }
  }
}
