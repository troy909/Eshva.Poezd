#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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
  [ExcludeFromCodeCoverage]
  internal sealed class EmptyBrokerEgressDriver : IBrokerEgressDriver
  {
    /// <inheritdoc />
    public void Initialize(
      string brokerId,
      IEnumerable<IEgressApi> apis,
      IDiContainerAdapter serviceProvider) { }

    /// <inheritdoc />
    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken) => Task.CompletedTask;

    /// <inheritdoc />
    public void Dispose() { }
  }
}
