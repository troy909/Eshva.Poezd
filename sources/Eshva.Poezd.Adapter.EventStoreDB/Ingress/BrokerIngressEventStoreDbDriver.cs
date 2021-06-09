#region Usings

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
    public BrokerIngressEventStoreDbDriver(BrokerIngressEventStoreDbDriverConfiguration configuration)
    {
      _configuration = configuration;
    }

    public void Dispose()
    {
      // TODO: Dispose or close all subscriptions.
    }

    public void Initialize(
      IBrokerIngress brokerIngress,
      IEnumerable<IIngressApi> apis,
      IDiContainerAdapter serviceProvider)
    {
      // TODO: Initialize the driver.
    }

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default) =>
      // TODO: Subscribe to subscription groups.
      Task.CompletedTask;

    private readonly BrokerIngressEventStoreDbDriverConfiguration _configuration;
  }
}
