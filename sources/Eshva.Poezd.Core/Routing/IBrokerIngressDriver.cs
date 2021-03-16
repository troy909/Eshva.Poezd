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
  public interface IBrokerIngressDriver : IDisposable
  {
    /// <summary>
    /// Initializes the message broker driver.
    /// </summary>
    /// <param name="brokerId">
    /// The broker ID to bind to.
    /// </param>
    /// <param name="messageRouter">
    /// Message router to bind to.
    /// </param>
    /// <param name="apiConfigurations"></param>
    /// <param name="serviceProvider">
    /// The service provider.
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
    void Initialize(
      [NotNull] string brokerId,
      [NotNull] IMessageRouter messageRouter,
      [NotNull] IEnumerable<IIngressApi> apis,
      [NotNull] IServiceProvider serviceProvider);

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
    Task StartConsumeMessages(
      [NotNull] IEnumerable<string> queueNamePatterns,
      CancellationToken cancellationToken = default);
  }
}
