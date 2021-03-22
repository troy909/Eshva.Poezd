#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Contract of message broker.
  /// </summary>
  /// <remarks>
  /// Message broker represents a broker server/endpoint.
  /// </remarks>
  public interface IMessageBroker : IDisposable
  {
    /// <summary>
    /// Gets the message broker ID.
    /// </summary>
    string Id { get; }

    /// <summary>
    /// Gets the message broker ingress.
    /// </summary>
    IBrokerIngress Ingress { get; }

    /// <summary>
    /// Gets the message broker egress.
    /// </summary>
    IBrokerEgress Egress { get; }

    /// <summary>
    /// The message broker configuration.
    /// </summary>
    MessageBrokerConfiguration Configuration { get; }

    /// <summary>
    /// Initializes the message broker ingress and/or egress before it could be used.
    /// </summary>
    void Initialize();

    /// <summary>
    /// Starts message consuming.
    /// </summary>
    /// <param name="queueNamePatterns">
    /// The queue/topic name patters this broker should subscribe to.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting when message consuming finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The queue/topic name patterns not specified.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// List of patterns contains no patterns.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// The driver is not initialized yet.
    /// </exception>
    [NotNull]
    Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default);

    /// <summary>
    /// Publishes a message.
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
    Task Publish(MessagePublishingContext context, CancellationToken cancellationToken);

    /// <summary>
    /// Route an ingress message to message handlers.
    /// </summary>
    /// <param name="queueName">
    /// The queue/topic name from which the message is arrived.
    /// </param>
    /// <param name="receivedOnUtc">
    /// The moment in time the message was received.
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
    /// <exception cref="PoezdConfigurationException">
    /// An error occurred during ingress message handling.
    /// </exception>
    [NotNull]
    Task RouteIngressMessage(
      string queueName,
      DateTimeOffset receivedOnUtc,
      object key,
      object payload,
      IReadOnlyDictionary<string, string> metadata);
  }
}
