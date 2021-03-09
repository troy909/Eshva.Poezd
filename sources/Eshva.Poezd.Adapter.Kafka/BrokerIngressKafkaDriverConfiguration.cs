#region Usings

using System;
using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class BrokerIngressKafkaDriverConfiguration
  {
    public ConsumerConfig ConsumerConfig { get; internal set; }

    public Type HeaderValueParserType { get; internal set; }
  }
}
