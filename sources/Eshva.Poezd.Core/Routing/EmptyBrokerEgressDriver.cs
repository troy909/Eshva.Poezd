#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public class EmptyBrokerEgressDriver : IBrokerEgressDriver
  {
    public void Dispose() { }

    public void Initialize(string brokerId, ILogger<IBrokerEgressDriver> logger) { }

    public Task Publish(
      byte[] key,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames) => Task.CompletedTask;
  }
}
