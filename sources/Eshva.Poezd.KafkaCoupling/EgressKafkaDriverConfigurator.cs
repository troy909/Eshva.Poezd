using System;
using Confluent.Kafka;
using JetBrains.Annotations;

namespace Eshva.Poezd.KafkaCoupling
{
  public class EgressKafkaDriverConfigurator
  {
    public EgressKafkaDriverConfigurator(EgressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    public EgressKafkaDriverConfigurator WithProducerConfig([NotNull] ProducerConfig producerConfig)
    {
      _configuration.ProducerConfig = producerConfig ?? throw new ArgumentNullException(nameof(producerConfig));
      return this;
    }

    private readonly EgressKafkaDriverConfiguration _configuration;
  }
}