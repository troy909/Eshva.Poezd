#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  internal sealed class EmptyBrokerIngressDriver : IBrokerIngressDriver
  {
    public void Dispose() { }

    public void Initialize(
      IMessageRouter messageRouter,
      string brokerId,
      IServiceProvider serviceProvider) { }

    public Task StartConsumeMessages(
      IEnumerable<string> queueNamePatterns,
      CancellationToken cancellationToken = default) => Task.CompletedTask;
  }
}
