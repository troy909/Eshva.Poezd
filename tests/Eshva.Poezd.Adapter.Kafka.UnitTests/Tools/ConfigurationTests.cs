#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Core.Routing;
using Moq;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests.Tools
{
  public static class ConfigurationTests
  {
    public static BrokerEgressKafkaDriverConfiguration CreateBrokerEgressKafkaDriverConfiguration(
      List<MessagePublishingContext> publishedMessages = default,
      Exception exceptionToThrowOnPublishing = default) =>
      new BrokerEgressKafkaDriverConfiguration
      {
        ProducerConfig = new ProducerConfig(),
        ProducerFactoryType = typeof(ApiProducerFactory),
        HeaderValueCodecType = typeof(HeaderValueCodec),
        ProducerConfiguratorType = typeof(ProducerConfigurator),
        SerializerFactoryType = typeof(SerializerFactory)
      };

    public static BrokerEgressKafkaDriverConfiguration CreateBrokerEgressKafkaDriverConfigurationWithout(
      Action<BrokerEgressKafkaDriverConfiguration> updater)
    {
      var configuration = CreateBrokerEgressKafkaDriverConfiguration();
      updater(configuration);
      return configuration;
    }

    public static BrokerIngressKafkaDriverConfiguration CreateBrokerIngressKafkaDriverConfiguration() =>
      new BrokerIngressKafkaDriverConfiguration
      {
        ConsumerConfig = new ConsumerConfig(),
        ConsumerConfiguratorType = typeof(object),
        ConsumerFactoryType = typeof(object),
        DeserializerFactoryType = typeof(object),
        HeaderValueCodecType = typeof(object)
      };

    public static BrokerIngressKafkaDriverConfiguration CreateBrokerIngressKafkaDriverConfigurationWithout(
      Action<BrokerIngressKafkaDriverConfiguration> updater)
    {
      var configuration = CreateBrokerIngressKafkaDriverConfiguration();
      updater(configuration);
      return configuration;
    }

    public class SerializerFactory : ISerializerFactory
    {
      public ISerializer<TData> Create<TData>() => (ISerializer<TData>) Serializers.ByteArray;
    }

    public class ProducerConfigurator : IProducerConfigurator
    {
      public ProducerBuilder<TKey, TValue> Configure<TKey, TValue>(
        ProducerBuilder<TKey, TValue> builder,
        ISerializer<TKey> keySerializer,
        ISerializer<TValue> valueSerializer) =>
        builder;
    }

    public class HeaderValueCodec : IHeaderValueCodec
    {
      public string Decode(byte[] value) => string.Empty;

      public byte[] Encode(string value) => new byte[0];
    }

    public class ProducerFactory : IProducerFactory
    {
      public IProducer<TKey, TValue> Create<TKey, TValue>(ProducerConfig config) => Mock.Of<IProducer<TKey, TValue>>();
    }

    public class ApiProducerFactory : IApiProducerFactory
    {
      public IApiProducer Create<TKey, TValue>(ProducerConfig config) => null;
    }
  }
}
