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
      {
        yield return "ID of ingress API should be specified. " +
                     $"Use {nameof(IngressApiConfigurator)}.{nameof(IngressApiConfigurator.WithId)} " +
                     "to set the API ID.";
      }

      if (PipeFitterType == null)
      {
        yield return $"The ingress pipe fitter type should be set for the API with ID '{Id}'. " +
                     $"Use {nameof(IngressApiConfigurator)}.{nameof(IngressApiConfigurator.WithPipeFitter)} " +
                     "to set the pipe fitter type.";
      }

      if (HandlerRegistryType == null)
      {
        yield return $"The handler registry type should be set for the API with ID '{Id}'. " +
                     $"Use {nameof(IngressApiConfigurator)}.{nameof(IngressApiConfigurator.WithHandlerRegistry)} " +
                     "to set the handler registry type.";
      }

      if (QueueNamePatternsProviderType == null)
      {
        yield return $"The queue name patterns provider type should be set for the API with ID '{Id}'. " +
                     $"Use {nameof(IngressApiConfigurator)}.{nameof(IngressApiConfigurator.WithQueueNamePatternsProvider)} " +
                     "to set the queue name patterns provider type.";
      }

      if (MessageTypesRegistryType == null)
      {
        yield return $"The message types registry type should be set for the API with ID '{Id}'. " +
                     $"Use {nameof(IngressApiConfigurator)}.{nameof(IngressApiConfigurator.WithMessageTypesRegistry)} " +
                     "to set the message types registry type.";
      }

      if (MessageKeyType == null)
      {
        yield return $"The message key type should be set for the API with ID '{Id}'. " +
                     $"Use {nameof(IngressApiConfigurator)}.{nameof(IngressApiConfigurator.WithMessageKey)} " +
                     "to set the message key type.";
      }

      if (MessagePayloadType == null)
      {
        yield return $"The message payload type should be set for the API with ID '{Id}'. " +
                     $"Use {nameof(IngressApiConfigurator)}.{nameof(IngressApiConfigurator.WithMessagePayload)} " +
                     "to set the message payload type.";
      }
    }
  }
}
