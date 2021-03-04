#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.KafkaCoupling
{
  public static class IngressConfiguratorExtensions
  {
    public static IngressConfigurator WithKafkaDriver(
      this IngressConfigurator ingress,
      Action<IngressKafkaDriverConfigurator> configurator)
    {
      var configuration = new IngressKafkaDriverConfiguration();
      configurator(new IngressKafkaDriverConfigurator(configuration));
      ((IIngressDriverConfigurator) ingress).Driver = new IngressKafkaDriver(configuration);
      return ingress;
    }
  }
}
