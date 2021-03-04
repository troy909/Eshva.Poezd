using System;
using Confluent.Kafka;
using JetBrains.Annotations;

namespace Eshva.Poezd.KafkaCoupling
{
  public class IngressKafkaDriverConfigurator
  {
    public IngressKafkaDriverConfigurator(IngressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IngressKafkaDriverConfigurator WithConsumerConfig([NotNull] ConsumerConfig consumerConfig)
    {
      _configuration.ConsumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));
      return this;
    }

    public IngressKafkaDriverConfigurator WithHeaderValueParser<THeaderValueParser>()
    {
      _configuration.HeaderValueParserType = typeof(THeaderValueParser);
      return this;
    }

    private readonly IngressKafkaDriverConfiguration _configuration;
  }
}