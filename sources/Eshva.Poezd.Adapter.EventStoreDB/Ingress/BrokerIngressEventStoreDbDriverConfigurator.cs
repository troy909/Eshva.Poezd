#region Usings

using System;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  public class BrokerIngressEventStoreDbDriverConfigurator
  {
    public BrokerIngressEventStoreDbDriverConfigurator(BrokerIngressEventStoreDbDriverConfiguration configuration)
    {
      throw new NotImplementedException();
    }

    public BrokerIngressEventStoreDbDriverConfigurator WithConnection(EventStoreDbConnectionConfiguration connectionConfiguration) =>
      throw new NotImplementedException();

    public BrokerIngressEventStoreDbDriverConfigurator WithHeaderValueCodec<T>() => throw new NotImplementedException();
  }
}
