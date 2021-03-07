#region Usings

using System;
using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public class BrokerIngressKafkaDriverConfigurator
  {
    public BrokerIngressKafkaDriverConfigurator(BrokerIngressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    public BrokerIngressKafkaDriverConfigurator WithConsumerConfig([NotNull] ConsumerConfig consumerConfig)
    {
      _configuration.ConsumerConfig = consumerConfig ?? throw new ArgumentNullException(nameof(consumerConfig));
      return this;
    }

    public BrokerIngressKafkaDriverConfigurator WithHeaderValueParser<THeaderValueParser>()
    {
      _configuration.HeaderValueParserType = typeof(THeaderValueParser);
      return this;
    }

    private readonly BrokerIngressKafkaDriverConfiguration _configuration;
  }
}
