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
    IBrokerIngressDriver Driver { get; }

    /// <summary>
    /// Gets list of ingress APIs bound to this message broker.
    /// </summary>
    [NotNull]
    IReadOnlyCollection<IIngressApi> Apis { get; }

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
    /// Gets an ingress API by queue name.
    /// </summary>
    /// <param name="queueName">
    /// Queue name that should belong to one of ingress APIs bound to this broker.
    /// </param>
    /// <returns>
    /// The ingress API to which queue name belongs or a stab ingress API for an unknown queue name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Queue name is null, an empty or a whitespace string.
    /// </exception>
    IIngressApi GetApiByQueueName([NotNull] string queueName);

    /// <summary>
    /// Initializes the message broker driver.
    /// </summary>
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
