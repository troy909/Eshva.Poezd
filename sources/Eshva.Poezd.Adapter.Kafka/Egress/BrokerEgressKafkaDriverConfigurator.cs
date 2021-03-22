#region Usings

using System;
using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Broker egress Kafka driver configurator.
  /// </summary>
  [PublicAPI]
  public class BrokerEgressKafkaDriverConfigurator
  {
    /// <summary>
    /// Constructs a new instance of configurator.
    /// </summary>
    /// <param name="configuration">
    /// The egress Kafka driver configuration to configure with this configurator.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The egress Kafka driver configuration is not specified.
    /// </exception>
    public BrokerEgressKafkaDriverConfigurator([NotNull] BrokerEgressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets Kafka producer configuration.
    /// </summary>
    public BrokerEgressKafkaDriverConfigurator WithProducerConfig([NotNull] ProducerConfig producerConfig)
    {
      _configuration.ProducerConfig = producerConfig ?? throw new ArgumentNullException(nameof(producerConfig));
      return this;
    }

    /// <summary>
    /// Sets the producer factory type.
    /// </summary>
    /// <remarks>
    /// The producer factory used to create instances of <see cref="IProducer{TKey,TValue}" />.
    /// </remarks>
    public BrokerEgressKafkaDriverConfigurator WithProducerFactory<TProducerFactory>() where TProducerFactory : IProducerFactory
    {
      _configuration.ProducerFactoryType = typeof(TProducerFactory);
      return this;
    }

    /// <summary>
    /// Sets the producer configurator type.
    /// </summary>
    /// <remarks>
    /// The producer configurator used to call <see cref="ProducerBuilder{TKey,TValue}" /> methods like SetKeySerializer,
    /// SetValueSerializer etc.
    /// </remarks>
    public BrokerEgressKafkaDriverConfigurator WithProducerConfigurator<TProducerConfigurator>()
      where TProducerConfigurator : IProducerConfigurator
    {
      _configuration.ProducerConfiguratorType = typeof(TProducerConfigurator);
      return this;
    }

    /// <summary>
    /// Sets the serializer factory type.
    /// </summary>
    /// <remarks>
    /// The serializer factory used to create message keys and payloads serializers.
    /// </remarks>
    public BrokerEgressKafkaDriverConfigurator WithSerializerFactory<TSerializerFactory>() where TSerializerFactory : ISerializerFactory
    {
      _configuration.SerializerFactoryType = typeof(TSerializerFactory);
      return this;
    }

    /// <summary>
    /// Sets the header value codec type.
    /// </summary>
    /// <remarks>
    /// Kafka handles header values as byte arrays. Poezd handles broker headers as strings. This parser should translate bytes
    /// to string in accordance the way headers are encoded on producer side.
    /// </remarks>
    public BrokerEgressKafkaDriverConfigurator WithHeaderValueCodec<THeaderValueEncoder>() where THeaderValueEncoder : IHeaderValueCodec
    {
      _configuration.HeaderValueCodecType = typeof(THeaderValueEncoder);
      return this;
    }

    /// <summary>
    /// Sets the default implementation of producer factory.
    /// </summary>
    /// <remarks>
    /// See <see cref="DefaultProducerFactory" /> for details.
    /// </remarks>
    public BrokerEgressKafkaDriverConfigurator WithDefaultProducerFactory()
    {
      _configuration.ProducerFactoryType = typeof(DefaultProducerFactory);
      return this;
    }

    private readonly BrokerEgressKafkaDriverConfiguration _configuration;
  }
}
