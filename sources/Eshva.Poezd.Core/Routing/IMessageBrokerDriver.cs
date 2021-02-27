#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Contract of message broker driver.
  /// </summary>
  /// <remarks>
  /// Message broker driver is a bridge between message broker client and Poezd router.
  /// </remarks>
  public interface IMessageBrokerDriver : IDisposable
  {
    /// <summary>
    /// Initializes the message broker driver.
    /// </summary>
    /// <param name="messageRouter">
    /// Message router to bind to.
    /// </param>
    /// <param name="brokerId">
    /// The broker ID to bind to.
    /// </param>
    /// <param name="configuration">
    /// The driver configuration.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is null, an empty or whitespace string.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// The configuration object has wrong type.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// The driver is already initialized.
    /// </exception>
    public void Initialize(
      [NotNull] IMessageRouter messageRouter,
      [NotNull] string brokerId,
      [NotNull] object configuration);

    /// <summary>
    /// Starts message consuming from the broker client.
    /// </summary>
    /// <param name="queueNamePatterns">
    /// List of queue name patterns to subscribe to.
    /// </param>
    /// <param name="cancellationToken">
    /// Cancellation token used to finish message consumption.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting when message consuming finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Queue name patterns are not specified.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// List of patterns contains no patterns.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// The driver is not yet initialized.
    /// </exception>
    [NotNull]
    Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default);

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
    /// <param name="queueName">
    /// The message broker queue/topic to which the message should be published.
    /// </param>
    /// <returns>
    /// A task that can be used for waiting when publishing finished.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    [NotNull]
    Task Publish(
      string key,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      string queueName);
  }
}
