#region Usings

using System;
using System.Collections.Generic;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class PublicApiConfiguration : IMessageRouterConfigurationPart
  {
    public string Id { get; internal set; }

    public Type IngressPipelineConfiguratorType { get; set; }

    public Type HandlerFactoryType { get; set; }

    public Type QueueNamePatternsProviderType { get; set; }

    public IEnumerable<string> Validate()
    {
      if (string.IsNullOrWhiteSpace(Id)) yield return "Public API ID should be set.";
      if (IngressPipelineConfiguratorType == null) yield return "Pipeline configurator type should be set.";
      if (HandlerFactoryType == null) yield return "Handler factory type should be set.";
      if (QueueNamePatternsProviderType == null) yield return "Queue name patterns provider type should be set.";
    }
  }
}
