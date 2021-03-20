#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Configuration of an egress API.
  /// </summary>
  public class EgressApiConfiguration : IMessageRouterConfigurationPart
  {
    /// <summary>
    /// Gets the ID of the egress API.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the type of pipe fitter that sets up the pipeline used to prepare published messages.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IPipeFitter" />.
    /// </remarks>
    public Type PipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the type of message key.
    /// </summary>
    /// <remarks>
    /// The key required only for some message broker. For instance Kafka uses it to select partition of a topic to place a
    /// message to. This type used to select proper serializer for the message key.
    /// </remarks>
    public Type MessageKeyType { get; internal set; }

    /// <summary>
    /// Gets the type of message payload.
    /// </summary>
    /// <remarks>
    /// This type used to select proper serializer for the message payload.
    /// </remarks>
    public Type MessagePayloadType { get; internal set; }

    /// <summary>
    /// Gets the type of message types registry.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IEgressMessageTypesRegistry" />.
    /// </remarks>
    public Type MessageTypesRegistryType { get; internal set; }

    /// <summary>
    /// An empty egress API configuration.
    /// </summary>
    public static EgressApiConfiguration Empty { get; } = CreateValidEmpty();

    /// <inheritdoc />
    public IEnumerable<string> Validate()
    {
      if (string.IsNullOrWhiteSpace(Id))
        yield return "ID of egress API should be specified.";
      if (PipeFitterType == null)
        yield return $"The egress pipe fitter type should be set for the API with ID '{Id}'.";
      if (MessageTypesRegistryType == null)
        yield return $"The message registry type should be set for the API with ID '{Id}'.";
    }

    private static EgressApiConfiguration CreateValidEmpty() => new()
    {
      Id = "empty egress API configuration",
      MessageTypesRegistryType = typeof(EmptyEgressMessageTypesRegistry),
      PipeFitterType = typeof(EmptyPipeFitter)
    };
  }
}
