#region Usings

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// An empty broker egress driver.
  /// </summary>
  internal sealed class EmptyBrokerEgressDriver : IBrokerEgressDriver
  {
    /// <inheritdoc />
    public void Dispose() { }

    /// <inheritdoc />
    public void Initialize(
      string brokerId,
      ILogger<IBrokerEgressDriver> logger,
      IClock clock,
      IEnumerable<IEgressApi> apis,
      IDiContainerAdapter serviceProvider) { }

    /// <inheritdoc />
    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken) => Task.CompletedTask;
  }
}
