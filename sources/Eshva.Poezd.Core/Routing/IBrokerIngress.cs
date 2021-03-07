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
  /// The contract of broker ingress.
  /// </summary>
  public interface IBrokerIngress : IDisposable
  {
    /// <summary>
    /// The message broker ingress configuration.
    /// </summary>
    [NotNull]
    BrokerIngressConfiguration Configuration { get; }

    /// <summary>
    /// Gets the message broker ingress driver.
    /// </summary>
    [NotNull]
    IIngressDriver Driver { get; }

    /// <summary>
    /// Gets list of public APIs bound to this message broker.
    /// </summary>
    [NotNull]
    IReadOnlyCollection<IIngressPublicApi> PublicApis { get; }

    /// <summary>
    /// Gets enter pipe fitter. Configures the very beginning of pipeline.
    /// </summary>
    [NotNull]
    IPipeFitter EnterPipeFitter { get; }

    /// <summary>
    /// Gets exit pipe fitter. Configures the very end of pipeline.
    /// </summary>
    [NotNull]
    IPipeFitter ExitPipeFitter { get; }

    /// <summary>
    /// Gets public API by queue name.
    /// </summary>
    /// <param name="queueName">
    /// Queue name that should belong to one of public APIs bound to this broker.
    /// </param>
    /// <returns>
    /// Public API to which queue name belongs or a stab public API for an unknown queue name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Queue name is null, an empty or a whitespace string.
    /// </exception>
    IIngressPublicApi GetApiByQueueName([NotNull] string queueName);

    /// <summary>
    /// Initializes the message broker driver.
    /// </summary>
    /// <param name="messageRouter">
    /// Message router to bind to.
    /// </param>
    /// <param name="brokerId">
    /// The broker ID to bind to.
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
    public Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default);

    void Initialize([NotNull] IMessageRouter messageRouter, [NotNull] string brokerId);
  }
}
