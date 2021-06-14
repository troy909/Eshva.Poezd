#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.EventStore.Ingress
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithEventStoreDbDriver(
      this BrokerIngressConfigurator brokerIngress,
      [NotNull] Action<BrokerIngressEventStoreDriverConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new BrokerIngressEventStoreDriverConfiguration();
      configurator(new BrokerIngressEventStoreDriverConfigurator(configuration));
      IBrokerIngressDriverConfigurator driverConfigurator = brokerIngress;
      driverConfigurator.SetDriver(
        new BrokerIngressEventStoreDriver(configuration, new DefaultStreamSubscriptionRegistry()),
        configuration);
      return brokerIngress;
    }
  }
}
