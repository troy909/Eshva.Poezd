#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Contract of a public API.
  /// </summary>
  public interface IPublicApi
  {
    /// <summary>
    /// Gets the public API ID.
    /// </summary>
    [NotNull]
    string Id { get; }

    /// <summary>
    /// Gets the public API configuration.
    /// </summary>
    [NotNull]
    PublicApiConfiguration Configuration { get; }

    /// <summary>
    /// Gets the ingress pipe fitter.
    /// </summary>
    [NotNull]
    IPipeFitter IngressPipeFitter { get; }

    /// <summary>
    /// Gets the message type registry.
    /// </summary>
    [NotNull]
    IMessageTypesRegistry MessageTypesRegistry { get; }

    /// <summary>
    /// Gets queue name patterns.
    /// </summary>
    /// <returns>
    /// List of queue name patterns.
    /// </returns>
    [NotNull]
    IEnumerable<string> GetQueueNamePatterns();
  }
}
