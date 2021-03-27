#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// Broker ingress Kafka driver configuration.
  /// </summary>
  public class BrokerIngressKafkaDriverConfiguration : IMessageRouterConfigurationPart
  {
    /// <summary>
    /// Gets Kafka consumer configuration.
    /// </summary>
    public ConsumerConfig ConsumerConfig { get; internal set; }

    /// <summary>
    /// Gets the API consumer factory type.
    /// </summary>
    public Type ConsumerFactoryType { get; internal set; }

    /// <summary>
    /// Gets the consumer configurator type.
    /// </summary>
    public Type ConsumerConfiguratorType { get; internal set; }

    /// <summary>
    /// Gets the deserializer factory type.
    /// </summary>
    public Type DeserializerFactoryType { get; internal set; }

    /// <summary>
    /// Gets the header value codec type.
    /// </summary>
    public Type HeaderValueCodecType { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string> Validate()
    {
      if (ConsumerConfig == null) yield return "Consumer configuration for broker ingress Kafka driver should be specified.";
      if (HeaderValueCodecType == null) yield return "Header value parser type for broker ingress Kafka driver should be specified.";
      if (ConsumerConfiguratorType == null) yield return "Consumer configurator type for broker ingress Kafka driver should be specified.";
      if (DeserializerFactoryType == null) yield return "Deserializer factory type for broker ingress Kafka driver should be specified.";
      if (ConsumerFactoryType == null) yield return "Consumer factory type for broker ingress Kafka driver should be specified.";
    }
  }
}
