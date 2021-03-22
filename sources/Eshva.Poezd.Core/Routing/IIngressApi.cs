#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Contract of an ingress API.
  /// </summary>
  public interface IIngressApi
  {
    /// <summary>
    /// Gets the ingress API ID.
    /// </summary>
    [NotNull]
    string Id { get; }

    /// <summary>
    /// Gets the ingress API configuration.
    /// </summary>
    [NotNull]
    IngressApiConfiguration Configuration { get; }

    /// <summary>
    /// Gets the ingress pipe fitter.
    /// </summary>
    [NotNull]
    IPipeFitter PipeFitter { get; }

    /// <summary>
    /// Gets the message types registry.
    /// </summary>
    [NotNull]
    IIngressApiMessageTypesRegistry MessageTypesRegistry { get; }

    /// <summary>
    /// Gets the handlers registry.
    /// </summary>
    [NotNull]
    IHandlerRegistry HandlerRegistry { get; }

    /// <summary>
    /// Gets the message key type.
    /// </summary>
    /// <remarks>
    /// The key required only for some message broker. For instance Kafka uses it to select partition of a topic to place a
    /// message to. This type used to select proper deserializer for the message key.
    /// </remarks>
    [NotNull]
    Type MessageKeyType { get; }

    /// <summary>
    /// Gets the message payload type.
    /// </summary>
    /// <remarks>
    /// This type used to select proper deserializer for the message payload.
    /// </remarks>
    [NotNull]
    Type MessagePayloadType { get; }

    /// <summary>
    /// Gets queue name patterns belonged to this ingress API.
    /// </summary>
    /// <returns>
    /// List of queue name patterns.
    /// </returns>
    [NotNull]
    IEnumerable<string> GetQueueNamePatterns();
  }
}
