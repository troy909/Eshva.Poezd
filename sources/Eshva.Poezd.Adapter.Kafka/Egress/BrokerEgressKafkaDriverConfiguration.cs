#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Broker egress Kafka driver configuration.
  /// </summary>
  public class BrokerEgressKafkaDriverConfiguration : IMessageRouterConfigurationPart

  {
    /// <summary>
    /// Gets Kafka producer configuration.
    /// </summary>
    public ProducerConfig ProducerConfig { get; internal set; }

    /// <summary>
    /// Gets the producer factory type.
    /// </summary>
    public Type ProducerFactoryType { get; internal set; }

    /// <summary>
    /// Gets the producer configurator type.
    /// </summary>
    public Type ProducerConfiguratorType { get; internal set; }

    /// <summary>
    /// Gets the serializer factory type.
    /// </summary>
    public Type SerializerFactoryType { get; internal set; }

    /// <summary>
    /// Gets the header value codec type.
    /// </summary>
    public Type HeaderValueCodecType { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string> Validate()
    {
      if (ProducerConfig == null)
        yield return "Producer configuration for broker egress Kafka driver should be specified.";
      if (ProducerFactoryType == null)
        yield return "Producer factory type for broker egress Kafka driver should be specified.";
      if (ProducerConfiguratorType == null)
        yield return "Producer configurator type for broker egress Kafka driver should be specified.";
      if (SerializerFactoryType == null)
        yield return "Serializer factory type for broker egress Kafka driver should be specified.";
      if (HeaderValueCodecType == null)
        yield return "Header value codec type for broker egress Kafka driver should be specified.";
    }
  }
}
