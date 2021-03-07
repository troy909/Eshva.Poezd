#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class IngressPublicApiConfiguration : IMessageRouterConfigurationPart
  {
    public string Id { get; set; }

    public Type QueueNamePatternsProviderType { get; set; }

    public Type PipeFitterType { get; set; }

    public Type HandlerRegistryType { get; set; }

    public Type MessageTypesRegistryType { get; set; }

    public static IngressPublicApiConfiguration Empty { get; } = CreateValidEmpty();

    /// <inheritdoc />
    public IEnumerable<string> Validate()
    {
      if (string.IsNullOrWhiteSpace(Id))
        yield return "ID of the public API should be specified.";
      if (PipeFitterType == null)
        yield return $"The ingress pipe fitter type should be set for the public API with ID '{Id}'.";
      if (HandlerRegistryType == null)
        yield return $"The handler factory type should be set for the public API with ID '{Id}'.";
      if (QueueNamePatternsProviderType == null)
        yield return $"The queue name patterns provider type should be set for the public API with ID '{Id}'.";
      if (MessageTypesRegistryType == null)
        yield return $"The message registry type should be set for the public API with ID '{Id}'.";
    }

    private static IngressPublicApiConfiguration CreateValidEmpty() => new()
    {
      Id = "empty ingress public API configuration",
      HandlerRegistryType = typeof(EmptyHandlerRegistry),
      MessageTypesRegistryType = typeof(EmptyIngressMessageTypesRegistry),
      PipeFitterType = typeof(EmptyPipeFitter),
      QueueNamePatternsProviderType = typeof(ProvidingNothingQueueNamePatternsProvider)
    };
  }
}
