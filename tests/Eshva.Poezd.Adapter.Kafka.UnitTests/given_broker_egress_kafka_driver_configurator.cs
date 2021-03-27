#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.UnitTests.Tools;
using FluentAssertions;
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
      sut.WithProducerFactory<ConfigurationTests.ApiProducerFactory>().Should().BeSameAs(sut);
      configuration.ProducerFactoryType.Should().Be(typeof(ConfigurationTests.ApiProducerFactory));
    }

    [Fact]
    public void when_default_producer_factory_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      var sut = new BrokerEgressKafkaDriverConfigurator(configuration);
      sut.WithDefaultProducerFactory().Should().BeSameAs(sut);
      configuration.ProducerFactoryType.Should().Be<DefaultApiProducerFactory>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_producer_config_set_with_invalid_arguments_it_should_fail()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      var configurator = new BrokerEgressKafkaDriverConfigurator(configuration);
      Action sut = () => configurator.WithProducerConfig(producerConfig: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void when_producer_configurator_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      var sut = new BrokerEgressKafkaDriverConfigurator(configuration);
      sut.WithProducerConfigurator<ConfigurationTests.ProducerConfigurator>().Should().BeSameAs(sut);
      configuration.ProducerConfiguratorType.Should().Be<ConfigurationTests.ProducerConfigurator>();
    }

    [Fact]
    public void when_serializer_factory_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      var sut = new BrokerEgressKafkaDriverConfigurator(configuration);
      sut.WithSerializerFactory<ConfigurationTests.SerializerFactory>().Should().BeSameAs(sut);
      configuration.SerializerFactoryType.Should().Be<ConfigurationTests.SerializerFactory>();
    }

    [Fact]
    public void when_header_value_codec_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerEgressKafkaDriverConfiguration();
      var sut = new BrokerEgressKafkaDriverConfigurator(configuration);
      sut.WithHeaderValueCodec<ConfigurationTests.HeaderValueCodec>().Should().BeSameAs(sut);
      configuration.HeaderValueCodecType.Should().Be<ConfigurationTests.HeaderValueCodec>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_construct_with_invalid_arguments_it_should_fail()
    {
      Action sut = () => new BrokerEgressKafkaDriverConfigurator(configuration: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }
  }
}
