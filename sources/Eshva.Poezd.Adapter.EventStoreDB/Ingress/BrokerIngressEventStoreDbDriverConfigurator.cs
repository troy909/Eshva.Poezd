#region Usings

using System;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  public class BrokerIngressEventStoreDbDriverConfigurator
  {
    public BrokerIngressEventStoreDbDriverConfigurator(BrokerIngressEventStoreDbDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    public BrokerIngressEventStoreDbDriverConfigurator WithConnection(EventStoreDbConnectionConfiguration connectionConfiguration)
    {
      _configuration.ConnectionConfiguration = connectionConfiguration;
      return this;
    }

    public BrokerIngressEventStoreDbDriverConfigurator WithHeaderValueCodec<THeaderValueCodec>() where THeaderValueCodec : IHeaderValueCodec
    {
      _configuration.HeaderValueCodecType = typeof(THeaderValueCodec);
      return this;
    }

    private readonly BrokerIngressEventStoreDbDriverConfiguration _configuration;
  }
}
