#region Usings

using System;
using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class BrokerEgressKafkaDriverConfigurator
  {
    public BrokerEgressKafkaDriverConfigurator(BrokerEgressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    public BrokerEgressKafkaDriverConfigurator WithProducerConfig([NotNull] ProducerConfig producerConfig)
    {
      _configuration.ProducerConfig = producerConfig ?? throw new ArgumentNullException(nameof(producerConfig));
      return this;
    }

    public BrokerEgressKafkaDriverConfigurator WithProducerConfigurator<TProducerConfigurator>()
      where TProducerConfigurator : IProducerConfigurator
    {
      _configuration.ProducerConfiguratorType = typeof(TProducerConfigurator);
      return this;
    }

    public BrokerEgressKafkaDriverConfigurator WithSerializerFactory<TSerializerFactory>() where TSerializerFactory : ISerializerFactory
    {
      _configuration.SerializerFactoryType = typeof(TSerializerFactory);
      return this;
    }

    public BrokerEgressKafkaDriverConfigurator WithHeaderValueCodec<THeaderValueEncoder>() where THeaderValueEncoder : IHeaderValueCodec
    {
      _configuration.HeaderValueCodecType = typeof(THeaderValueEncoder);
      return this;
    }

    public BrokerEgressKafkaDriverConfigurator WithProducerFactory<TProducerFactory>() where TProducerFactory : IProducerFactory
    {
      _configuration.ProducerFactoryType = typeof(TProducerFactory);
      return this;
    }

    public BrokerEgressKafkaDriverConfigurator WithDefaultProducerFactory()
    {
      _configuration.ProducerFactoryType = typeof(DefaultProducerFactory);
      return this;
    }

    private readonly BrokerEgressKafkaDriverConfiguration _configuration;
  }
}
