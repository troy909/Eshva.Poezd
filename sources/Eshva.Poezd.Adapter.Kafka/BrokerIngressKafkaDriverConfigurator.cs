#region Usings

using System;
using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  [PublicAPI]
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

    public BrokerIngressKafkaDriverConfigurator WithDeserializerFactory<TDeserializerFactory>()
      where TDeserializerFactory : IDeserializerFactory
    {
      _configuration.DeserializerFactoryType = typeof(TDeserializerFactory);
      return this;
    }

    public BrokerIngressKafkaDriverConfigurator WithConsumerFactory<TConsumerFactory>()
      where TConsumerFactory : IConsumerFactory
    {
      _configuration.ConsumerFactoryType = typeof(TConsumerFactory);
      return this;
    }

    public BrokerIngressKafkaDriverConfigurator WithConsumerConfigurator<TConsumerConfigurator>()
      where TConsumerConfigurator : IConsumerConfigurator
    {
      _configuration.ConsumerConfiguratorType = typeof(TConsumerConfigurator);
      return this;
    }

    public BrokerIngressKafkaDriverConfigurator WithHeaderValueCodec<THeaderValueCodec>()
      where THeaderValueCodec : IHeaderValueCodec
    {
      _configuration.HeaderValueParserType = typeof(THeaderValueCodec);
      return this;
    }

    private readonly BrokerIngressKafkaDriverConfiguration _configuration;
  }
}
