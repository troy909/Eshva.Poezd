#region Usings

using System;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// Extensions for <see cref="BrokerIngressConfigurator" />.
  /// </summary>
  [PublicAPI]
  public static class IngressConfiguratorExtensions
  {
    /// <summary>
    /// Sets Kafka driver as broker ingress driver.
    /// </summary>
    /// <param name="brokerIngress">
    /// The broker ingress.
    /// </param>
    /// <param name="configurator">
    /// The broker ingress Kafka driver configurator.
    /// </param>
    /// <returns>
    /// This broker ingress configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// This broker ingress configurator is not specified.
    /// </exception>
    public static BrokerIngressConfigurator WithKafkaDriver(
      this BrokerIngressConfigurator brokerIngress,
      [NotNull] Action<BrokerIngressKafkaDriverConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new BrokerIngressKafkaDriverConfiguration();
      configurator(new BrokerIngressKafkaDriverConfigurator(configuration));
      IBrokerIngressDriverConfigurator driverConfigurator = brokerIngress;
      driverConfigurator.SetDriver(new BrokerIngressKafkaDriver(configuration, new DefaultConsumerRegistry()), configuration);
      return brokerIngress;
    }
  }
}
