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
    /// Initializes the egress message broker driver.
    /// </summary>
    /// <param name="brokerId">
    /// The broker ID to bind to.
    /// </param>
    /// <param name="logger">
    /// A logger.
    /// </param>
    /// <param name="clock"></param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is null, an empty or whitespace string.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// The driver is already initialized.
    /// </exception>
    void Initialize(
      [NotNull] string brokerId,
      [NotNull] ILogger<IBrokerEgressDriver> logger,
      [NotNull] IClock clock,
      [NotNull] IEnumerable<IEgressApi> apis,
      [NotNull] IServiceProvider serviceProvider);

    /// <summary>
    /// Publishes a message using the broker client.
    /// </summary>
    /// <param name="key">
    /// The message broker key if required like in case of Kafka.
    /// </param>
    /// <param name="payload">
    /// The message broker payload.
    /// </param>
    /// <param name="api"></param>
    /// <param name="metadata">
    /// The message broker metadata.
    /// </param>
    /// <param name="queueNames">
    /// The message broker queue/topic names to which the message should be published.
    /// </param>
    /// <param name="cancellationToken"></param>
    /// <returns>
    /// A task that can be used for waiting when publishing finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    [NotNull]
    Task Publish([NotNull] MessagePublishingContext context, CancellationToken cancellationToken);
  }
}
