#region Usings

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  internal sealed class EmptyBrokerIngressDriver : IBrokerIngressDriver
  {
    public void Dispose() { }

    public void Initialize(
      IBrokerIngress brokerIngress,
      IEnumerable<IIngressApi> apis,
      IDiContainerAdapter serviceProvider) { }

    public Task StartConsumeMessages(
      IEnumerable<string> queueNamePatterns,
      CancellationToken cancellationToken = default) => Task.CompletedTask;
  }
}
