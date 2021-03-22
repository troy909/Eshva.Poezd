#region Usings

using System;
using System.Collections.ObjectModel;
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
  /// Contract of message broker egress.
  /// </summary>
  [PublicAPI]
  public interface IBrokerEgress : IDisposable
  {
    /// <summary>
    /// Gets the message broker egress configuration.
    /// </summary>
    [NotNull]
    BrokerEgressConfiguration Configuration { get; }

    /// <summary>
    /// Gets the message broker egress driver.
    /// </summary>
    [NotNull]
    IBrokerEgressDriver Driver { get; }

    /// <summary>
    /// Gets egress enter pipe fitter. Configures the very beginning of egress pipeline.
    /// </summary>
    [NotNull]
    IPipeFitter EnterPipeFitter { get; }

    /// <summary>
    /// Gets egress exit pipe fitter. Configures the very end of egress pipeline.
    /// </summary>
    [NotNull]
    IPipeFitter ExitPipeFitter { get; }

    /// <summary>
    /// Gets list of APIs bound to this message broker egress.
    /// </summary>
    [NotNull]
    ReadOnlyCollection<IEgressApi> Apis { get; }

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
    public void Initialize();

    /// <summary>
    /// Publishes a message to the message broker egress.
    /// </summary>
    /// <param name="context">
    /// Message publishing context containing everything to publish the message to the message broker egress.
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
