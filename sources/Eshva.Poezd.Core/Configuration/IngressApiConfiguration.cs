#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Configuration of an ingress API.
  /// </summary>
  public class IngressApiConfiguration : IMessageRouterConfigurationPart
  {
    /// <summary>
    /// Gets the ID of the ingress API.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the type of pipe fitter that sets up the pipeline used to handle received messages.
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
    /// message to. This type used to select proper deserializer for the message key.
    /// </remarks>
    public Type MessageKeyType { get; internal set; }

    /// <summary>
    /// Gets the type of message payload.
    /// </summary>
    /// <remarks>
    /// This type used to select proper deserializer for the message payload.
    /// </remarks>
    public Type MessagePayloadType { get; internal set; }

    /// <summary>
    /// Gets the type of message types registry.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IIngressApiMessageTypesRegistry" />.
    /// </remarks>
    public Type MessageTypesRegistryType { get; internal set; }

    /// <summary>
    /// Gets the type of queue name patterns provider.
    /// </summary>
    /// <remarks>
    /// This provider used to get queue names the API messages are published to. The format of these patterns depends on the
    /// message broker. For instance Kafka supports Regex patterns.
    /// </remarks>
    public Type QueueNamePatternsProviderType { get; internal set; }

    /// <summary>
    /// Gets the type of the message handlers registry.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IHandlerRegistry" />.
    /// </remarks>
    public Type HandlerRegistryType { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string> Validate()
    {
      if (string.IsNullOrWhiteSpace(Id))
        yield return "ID of ingress API should be specified.";
      if (PipeFitterType == null)
        yield return $"The ingress pipe fitter type should be set for the API with ID '{Id}'.";
      if (HandlerRegistryType == null)
        yield return $"The handler factory type should be set for the API with ID '{Id}'.";
      if (QueueNamePatternsProviderType == null)
        yield return $"The queue name patterns provider type should be set for the API with ID '{Id}'.";
      if (MessageTypesRegistryType == null)
        yield return $"The message registry type should be set for the API with ID '{Id}'.";
      if (MessageKeyType == null)
        yield return $"The message key type should be set for the API with ID '{Id}'.";
      if (MessagePayloadType == null)
        yield return $"The message payload type should be set for the API with ID '{Id}'.";
    }
  }
}
