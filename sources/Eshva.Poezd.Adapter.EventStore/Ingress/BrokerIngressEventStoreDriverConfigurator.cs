#region Usings

#endregion

namespace Eshva.Poezd.Adapter.EventStore.Ingress
{
  public class BrokerIngressEventStoreDriverConfigurator
  {
    public BrokerIngressEventStoreDriverConfigurator(BrokerIngressEventStoreDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    public BrokerIngressEventStoreDriverConfigurator WithConnection(EventStoreConnectionConfiguration connectionConfiguration)
    {
      _configuration.ConnectionConfiguration = connectionConfiguration;
      return this;
    }

    public BrokerIngressEventStoreDriverConfigurator WithHeaderValueCodec<THeaderValueCodec>() where THeaderValueCodec : IHeaderValueCodec
    {
      _configuration.HeaderValueCodecType = typeof(THeaderValueCodec);
      return this;
    }

    private readonly BrokerIngressEventStoreDriverConfiguration _configuration;
  }
}
