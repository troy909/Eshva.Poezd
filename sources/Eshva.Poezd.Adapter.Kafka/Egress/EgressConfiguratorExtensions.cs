#region Usings

using System;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Extensions for <see cref="BrokerEgressConfigurator" />.
  /// </summary>
  [PublicAPI]
  public static class EgressConfiguratorExtensions
  {
    /// <summary>
    /// Sets Kafka driver as broker egress driver.
    /// </summary>
    /// <param name="brokerEgress">
    /// The broker egress.
    /// </param>
    /// <param name="configurator">
    /// The broker egress Kafka driver configurator.
    /// </param>
    /// <returns>
    /// This broker egress configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// This broker egress configurator is not specified.
    /// </exception>
    public static BrokerEgressConfigurator WithKafkaDriver(
      this BrokerEgressConfigurator brokerEgress,
      [NotNull] Action<BrokerEgressKafkaDriverConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new BrokerEgressKafkaDriverConfiguration();
      configurator(new BrokerEgressKafkaDriverConfigurator(configuration));
      IBrokerEgressDriverConfigurator driverConfigurator = brokerEgress;
      driverConfigurator.SetDriver(new BrokerEgressKafkaDriver(configuration, new DefaultProducerRegistry()), configuration);
      return brokerEgress;
    }
  }
}
