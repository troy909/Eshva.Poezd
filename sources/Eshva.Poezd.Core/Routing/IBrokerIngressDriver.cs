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
    /// <param name="brokerIngress">
    /// The broker ingress.
    /// </param>
    /// <param name="apis">
    /// The list of ingress APIs bound to this driver.
    /// </param>
    /// <param name="serviceProvider">
    /// The service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    /// <exception cref="PoezdOperationException">
    /// The driver is already initialized.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// On of required service not found in DI-container.
    /// </exception>
    void Initialize(
      [NotNull] IBrokerIngress brokerIngress,
      [NotNull] IEnumerable<IIngressApi> apis,
      [NotNull] IDiContainerAdapter serviceProvider);

    /// <summary>
    /// Starts message consuming from this driver.
    /// </summary>
    /// <param name="queueNamePatterns">
    /// List of queue name patterns the driver should subscribe to.
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
    /// The driver is not initialized yet.
    /// </exception>
    [NotNull]
    Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default);
  }
}
