#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  public class BrokerIngressEventStoreDbDriver : IBrokerIngressDriver
  {
    public BrokerIngressEventStoreDbDriver(BrokerIngressEventStoreDbDriverConfiguration configuration) { }

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public void Initialize(
      IBrokerIngress brokerIngress,
      IEnumerable<IIngressApi> apis,
      IDiContainerAdapter serviceProvider)
    {
      throw new NotImplementedException();
    }

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default) =>
      throw new NotImplementedException();
  }
}
