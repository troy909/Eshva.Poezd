#region Usings

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  internal sealed class EmptyBrokerEgressDriver : IBrokerEgressDriver
  {
    public void Dispose() { }

    public void Initialize(
      string brokerId,
      ILogger<IBrokerEgressDriver> logger,
      IClock clock) { }

    public Task Publish(
      object key,
      object payload,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames,
      CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
