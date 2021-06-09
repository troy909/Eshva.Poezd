#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  public static class IngressConfiguratorExtensions
  {
    public static BrokerIngressConfigurator WithEventStoreDbDriver(
      this BrokerIngressConfigurator brokerIngress,
      [NotNull] Action<BrokerIngressEventStoreDbDriverConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new BrokerIngressEventStoreDbDriverConfiguration();
      configurator(new BrokerIngressEventStoreDbDriverConfigurator(configuration));
      IBrokerIngressDriverConfigurator driverConfigurator = brokerIngress;
      driverConfigurator.SetDriver(
        new BrokerIngressEventStoreDbDriver(configuration, new DefaultStreamSubscriptionRegistry()),
        configuration);
      return brokerIngress;
    }
  }
}
