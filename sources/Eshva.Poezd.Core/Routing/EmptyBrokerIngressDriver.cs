#region Usings

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// An empty broker ingress driver.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal sealed class EmptyBrokerIngressDriver : IBrokerIngressDriver
  {
    /// <inheritdoc />
    public void Initialize(
      IBrokerIngress brokerIngress,
      IEnumerable<IIngressApi> apis,
      IDiContainerAdapter serviceProvider) { }

    /// <inheritdoc />
    public Task StartConsumeMessages(
      IEnumerable<string> queueNamePatterns,
      CancellationToken cancellationToken = default) => Task.CompletedTask;

    /// <inheritdoc />
    public void Dispose() { }
  }
}
