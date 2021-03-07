#region Usings

using System;
using System.Collections.Generic;
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
    /// <exception cref="ArgumentNullException">
    /// One of arguments is null, an empty or whitespace string.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// The driver is already initialized.
    /// </exception>
    public void Initialize([NotNull] string brokerId, [NotNull] ILogger<IBrokerEgressDriver> logger);

    /// <summary>
    /// Publishes a message using the broker client.
    /// </summary>
    /// <param name="key">
    /// The message broker key if required like in case of Kafka.
    /// </param>
    /// <param name="payload">
    /// The message broker payload.
    /// </param>
    /// <param name="metadata">
    /// The message broker metadata.
    /// </param>
    /// <param name="queueNames">
    /// The message broker queue/topic names to which the message should be published.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting when publishing finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    [NotNull]
    Task Publish(
      byte[] key,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames);
  }
}
