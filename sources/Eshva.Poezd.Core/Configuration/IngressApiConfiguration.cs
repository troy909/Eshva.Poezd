#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class IngressApiConfiguration : IMessageRouterConfigurationPart
  {
    public string Id { get; set; }

    public Type QueueNamePatternsProviderType { get; set; }

    public Type MessageKeyType { get; set; }

    public Type MessagePayloadType { get; set; }

    public Type PipeFitterType { get; set; }

    public Type HandlerRegistryType { get; set; }

    public Type MessageTypesRegistryType { get; set; }

    public static IngressApiConfiguration Empty { get; } = CreateValidEmpty();

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

    private static IngressApiConfiguration CreateValidEmpty() => new()
    {
      Id = "empty ingress API configuration",
      HandlerRegistryType = typeof(EmptyHandlerRegistry),
      MessageTypesRegistryType = typeof(EmptyIngressMessageTypesRegistry),
      PipeFitterType = typeof(EmptyPipeFitter),
      QueueNamePatternsProviderType = typeof(ProvidingNothingQueueNamePatternsProvider),
      MessageKeyType = typeof(int),
      MessagePayloadType = typeof(byte[])
    };
  }
}
