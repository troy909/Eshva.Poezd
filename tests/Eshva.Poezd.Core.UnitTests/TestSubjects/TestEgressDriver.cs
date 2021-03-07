#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestEgressDriver : IBrokerEgressDriver
  {
    public void Dispose() { }

    public void Initialize(string brokerId, ILogger<IBrokerEgressDriver> logger) { }

    public Task Publish(
      byte[] key,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames) =>
      Task.CompletedTask;
  }
}
