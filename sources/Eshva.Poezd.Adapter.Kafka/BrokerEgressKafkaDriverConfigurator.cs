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

    public BrokerEgressKafkaDriverConfigurator WithProducerFactory(IProducerFactory producerFactory)
    {
      _configuration.ProducerFactory = producerFactory;
      return this;
    }

    public BrokerEgressKafkaDriverConfigurator WithDefaultProducerFactory()
    {
      _configuration.ProducerFactory = new DefaultProducerFactory();
      return this;
    }

    private readonly BrokerEgressKafkaDriverConfiguration _configuration;
  }
}
