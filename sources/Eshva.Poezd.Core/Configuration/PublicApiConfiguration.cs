#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /*
  /// <summary>
  /// Configuration of a public API.
  /// </summary>
  public sealed class PublicApiConfiguration : IMessageRouterConfigurationPart
  {
    /// <summary>
    /// Gets ID of the public API.
    /// </summary>
    public string Id { get; internal set; }

    /// <summary>
    /// Gets the ingress pipe fitter type.
    /// </summary>
    public Type IngressPipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the egress pipe fitter type.
    /// </summary>
    public Type EgressPipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the handler registry type.
    /// </summary>
    public Type HandlerRegistryType { get; internal set; }

    /// <summary>
    /// Gets the queue name patterns provider type.
    /// </summary>
    public Type QueueNamePatternsProviderType { get; internal set; }

    /// <summary>
    /// Gets the massage types registry type.
    /// </summary>
    public Type MessageTypesRegistryType { get; internal set; }

    /// <inheritdoc />
    public IEnumerable<string> Validate()
    {
      if (string.IsNullOrWhiteSpace(Id))
        yield return "ID of the public API should be specified.";
      if (IngressPipeFitterType == null)
        yield return $"The ingress pipe fitter type should be set for the public API with ID '{Id}'.";
      if (EgressPipeFitterType == null)
        yield return $"The egress pipe fitter type should be set for the public API with ID '{Id}'.";
      if (HandlerRegistryType == null)
        yield return $"The handler factory type should be set for the public API with ID '{Id}'.";
      if (QueueNamePatternsProviderType == null)
        yield return $"The queue name patterns provider type should be set for the public API with ID '{Id}'.";
      if (MessageTypesRegistryType == null)
        yield return $"The message registry type should be set for the public API with ID '{Id}'.";
    }
  }
*/
}
