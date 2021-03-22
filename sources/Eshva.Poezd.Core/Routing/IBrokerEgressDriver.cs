#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IBrokerEgressDriver : IDisposable
  {
    /// <summary>
    /// Initializes the message broker egress driver.
    /// </summary>
    /// <param name="brokerId">
    /// The broker ID to bind to.
    /// </param>
    /// <param name="logger">
    /// A logger.
    /// </param>
    /// <param name="clock">
    /// The service of the current time.
    /// </param>
    /// <param name="apis">
    /// The egress API list.
    /// </param>
    /// <param name="serviceProvider">
    /// The service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is null, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// The driver is already initialized.
    /// </exception>
    void Initialize(
      [NotNull] string brokerId,
      [NotNull] ILogger<IBrokerEgressDriver> logger,
      [NotNull] IClock clock,
      [NotNull] IEnumerable<IEgressApi> apis,
      [NotNull] IDiContainerAdapter serviceProvider);

    /// <summary>
    /// Publishes a message using the message broker driver.
    /// </summary>
    /// <param name="context">
    /// Message publishing context containing everything to publish the message to the message broker driver.
    /// </param>
    /// <param name="cancellationToken">
    /// A cancellation token.
    /// </param>
    /// <returns>
    /// A task that could be used for waiting when publishing finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Message publishing context is not specified.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// The driver is not initialized or the message publishing context missing some required data.
    /// </exception>
    [NotNull]
    Task Publish([NotNull] MessagePublishingContext context, CancellationToken cancellationToken);
  }
}
