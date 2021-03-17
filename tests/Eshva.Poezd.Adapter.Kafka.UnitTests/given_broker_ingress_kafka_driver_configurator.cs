#region Usings

using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  [SuppressMessage("ReSharper", "ClassNeverInstantiated.Local")]
  public class given_broker_ingress_kafka_driver_configurator
  {
    [Fact]
    public void when_consumer_configuration_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressKafkaDriverConfiguration();
      var sut = new BrokerIngressKafkaDriverConfigurator(configuration);
      var expected = new ConsumerConfig();
      sut.WithConsumerConfig(expected).Should().BeSameAs(sut);
      configuration.ConsumerConfig.Should().BeSameAs(expected);
    }

    [Fact]
    public void when_consumer_factory_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressKafkaDriverConfiguration();
      var sut = new BrokerIngressKafkaDriverConfigurator(configuration);
      sut.WithConsumerFactory<StabConsumerFactory>().Should().BeSameAs(sut);
      configuration.ConsumerFactoryType.Should().Be<StabConsumerFactory>();
    }

    [Fact]
    public void when_deserializer_factory_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressKafkaDriverConfiguration();
      var sut = new BrokerIngressKafkaDriverConfigurator(configuration);
      sut.WithDeserializerFactory<StabDeserializerFactory>().Should().BeSameAs(sut);
      configuration.DeserializerFactoryType.Should().Be<StabDeserializerFactory>();
    }

    [Fact]
    public void when_consumer_configurator_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressKafkaDriverConfiguration();
      var sut = new BrokerIngressKafkaDriverConfigurator(configuration);
      sut.WithConsumerConfigurator<StabConsumerConfigurator>().Should().BeSameAs(sut);
      configuration.ConsumerConfiguratorType.Should().Be<StabConsumerConfigurator>();
    }

    [Fact]
    public void when_header_value_parser_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressKafkaDriverConfiguration();
      var sut = new BrokerIngressKafkaDriverConfigurator(configuration);
      sut.WithHeaderValueCodec<StabHeaderValueCodec>().Should().BeSameAs(sut);
      configuration.HeaderValueCodecType.Should().Be<StabHeaderValueCodec>();
    }

    private class StabHeaderValueCodec : IHeaderValueCodec
    {
      public string Decode(byte[] value) => null;

      public byte[] Encode(string value) => null;
    }

    private class StabConsumerConfigurator : IConsumerConfigurator
    {
      public ConsumerBuilder<TKey, TValue> Configure<TKey, TValue>(
        ConsumerBuilder<TKey, TValue> builder,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer) =>
        null;
    }

    private class StabDeserializerFactory : IDeserializerFactory
    {
      public IDeserializer<TData> Create<TData>() => null;
    }

    private class StabConsumerFactory : IConsumerFactory
    {
      public IConsumer<TKey, TValue> Create<TKey, TValue>(
        ConsumerConfig consumerConfig,
        IConsumerConfigurator configurator,
        IDeserializerFactory deserializerFactory) =>
        null;
    }
  }
}
