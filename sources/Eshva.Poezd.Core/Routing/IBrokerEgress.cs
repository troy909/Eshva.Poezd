#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IBrokerEgress : IDisposable
  {
    /// <summary>
    /// The message broker egress configuration.
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
    ReadOnlyCollection<EgressApi> Apis { get; }

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
    public void Initialize([NotNull] IMessageRouter messageRouter, [NotNull] string brokerId);

    Task Publish(
      byte[] key,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames);
  }
}
