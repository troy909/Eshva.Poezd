#region Usings

using System;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Contract of an egress API.
  /// </summary>
  public interface IEgressApi
  {
    /// <summary>
    /// Gets the egress API ID.
    /// </summary>
    [NotNull]
    string Id { get; }

    /// <summary>
    /// Gets the egress API configuration.
    /// </summary>
    [NotNull]
    EgressApiConfiguration Configuration { get; }

    /// <summary>
    /// Gets the type of message key.
    /// </summary>
    /// <remarks>
    /// The key required only for some message broker. For instance Kafka uses it to select partition of a topic to place a
    /// message to. This type used to select proper serializer for the message key.
    /// </remarks>
    [NotNull]
    Type MessageKeyType => Configuration.MessageKeyType;

    /// <summary>
    /// Gets the type of message payload.
    /// </summary>
    /// <remarks>
    /// This type used to select proper serializer for the message payload.
    /// </remarks>
    [NotNull]
    Type MessagePayloadType => Configuration.MessagePayloadType;

    /// <summary>
    /// Gets the egress pipe fitter that sets up the pipeline used to prepare published messages.
    /// </summary>
    [NotNull]
    IPipeFitter PipeFitter { get; }

    /// <summary>
    /// Gets the message type registry.
    /// </summary>
    [NotNull]
    IEgressApiMessageTypesRegistry MessageTypesRegistry { get; }
  }
}
