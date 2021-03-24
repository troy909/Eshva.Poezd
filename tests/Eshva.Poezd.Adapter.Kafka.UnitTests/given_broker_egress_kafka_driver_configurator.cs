#region Usings

using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using FluentAssertions;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_broker_egress_kafka_driver_configurator
  {
    [Fact]
    public void when_producer_configuration_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      var sut = new BrokerEgressKafkaDriverConfigurator(configuration);
      var expected = new ProducerConfig();
      sut.WithProducerConfig(expected).Should().BeSameAs(sut);
      configuration.ProducerConfig.Should().BeSameAs(expected);
    }

    [Fact]
    public void when_producer_factory_type_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      var sut = new BrokerEgressKafkaDriverConfigurator(configuration);
      sut.WithProducerFactory<TestProducerFactory>().Should().BeSameAs(sut);
      configuration.ProducerFactoryType.Should().Be(typeof(TestProducerFactory));
    }

    [Fact]
    public void when_default_producer_factory_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      var sut = new BrokerEgressKafkaDriverConfigurator(configuration);
      sut.WithDefaultProducerFactory().Should().BeSameAs(sut);
      configuration.ProducerFactoryType.Should().Be<DefaultProducerFactory>();
    }

    private class TestProducerFactory : IProducerFactory
    {
      public IProducer<TKey, TValue> Create<TKey, TValue>(
        ProducerConfig config,
        IProducerConfigurator configurator,
        ISerializerFactory serializerFactory) => Mock.Of<IProducer<TKey, TValue>>();
    }
  }
}
