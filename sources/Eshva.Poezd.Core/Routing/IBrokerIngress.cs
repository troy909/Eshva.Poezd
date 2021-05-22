#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// The contract of the message broker ingress.
  /// </summary>
  [PublicAPI]
  public interface IBrokerIngress : IDisposable
  {
    /// <summary>
    /// Gets the message broker ingress configuration.
    /// </summary>
    [NotNull]
    BrokerIngressConfiguration Configuration { get; }

    /// <summary>
    /// Gets the message broker ingress driver.
    /// </summary>
    [NotNull]
    IBrokerIngressDriver Driver { get; }

    /// <summary>
    /// Gets the list of ingress APIs.
    /// </summary>
    [NotNull]
    IReadOnlyCollection<IIngressApi> Apis { get; }

    /// <summary>
    /// Gets enter pipe fitter which configures the very beginning of pipeline.
    /// </summary>
    [NotNull]
    IPipeFitter EnterPipeFitter { get; }

    /// <summary>
    /// Gets exit pipe fitter which configures the very end of pipeline.
    /// </summary>
    [NotNull]
    IPipeFitter ExitPipeFitter { get; }

    /// <summary>
    /// Initializes the message broker ingress.
    /// </summary>
    /// <returns>
    /// A task that could be used for waiting when message consumption finished.
    /// </returns>
    /// <exception cref="PoezdOperationException">
    /// The driver is already initialized.
    /// </exception>
    void Initialize();

    /// <summary>
    /// Starts message consumption from <paramref name="queueNamePatterns"/> by this broker ingress.
    /// </summary>
    /// <param name="queueNamePatterns">
    /// Queue name patterns to start message consumption from.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token that could be used to stop message consumption.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting the message consumption finished.
    /// </returns>
    [NotNull]
    public Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default);

    /// <summary>
    /// Routes an incoming message to message broker. Used by message broker drivers.
    /// </summary>
    /// <param name="queueName">
    /// The queue/topic name from which the message is arrived.
    /// </param>
    /// <param name="receivedOnUtc">
    /// The moment in time the message was received.
    /// TODO: Should be the original message timestamp?
    /// </param>
    /// <param name="key">
    /// The message key.
    /// </param>
    /// <param name="payload">
    /// The message payload.
    /// </param>
    /// <param name="metadata">
    /// The message metadata.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting the message routing finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is null, an empty or a whitespace string.
    /// </exception>
    [NotNull]
    Task RouteIngressMessage(
      string queueName,
      DateTimeOffset receivedOnUtc,
      object key,
      object payload,
      IReadOnlyDictionary<string, string> metadata);

    /// <summary>
    /// Gets ingress API by queue name.
    /// </summary>
    /// <param name="queueName">
    /// The queue name a message received from.
    /// </param>
    /// <returns>
    /// The ingress API to which queue name belongs or an empty ingress API for an unknown queue name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Queue name is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// <list type="bullet">
    /// <item>
    /// Queue name belongs to a few ingress APIs.
    /// </item>
    /// <item>
    /// Queue name doesn't belong to any ingress API.
    /// </item>
    /// </list>
    /// </exception>
    [NotNull]
    IIngressApi GetApiByQueueName([NotNull] string queueName);
  }
}
