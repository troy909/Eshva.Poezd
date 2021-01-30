#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class KafkaDriverConfiguration : IMessageRouterConfigurationPart
  {
    public ConsumerConfig ConsumerConfig { get; set; }

    public ProducerConfig ProducerConfig { get; set; }

    public long CommitPeriod { get; set; } = 1;

    public Type HeaderValueParserType { get; set; }

    public IEnumerable<string> Validate()
    {
      if (ConsumerConfig == null)
        yield return $"Kafka consumer config should be set. Use {nameof(KafkaDriverConfigurator.WithConsumerConfig)} to set one.";
      if (ProducerConfig == null)
        yield return $"Kafka producer config should be set. Use {nameof(KafkaDriverConfigurator.WithProducerConfig)} to set one.";
      if (HeaderValueParserType == null)
      {
        yield return $"Head value parser type isn't specified. Use {nameof(KafkaDriverConfigurator.WithHeaderValueParser)} with " +
                     "appropriate parser type to set one.";
      }

      if (CommitPeriod < 1)
        yield return $"Commit period should be greater than zero. Use {nameof(KafkaDriverConfigurator.WithCommitPeriod)} to set it.";
    }
  }
}
