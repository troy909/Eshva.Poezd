#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class PublicApiConfiguration : IMessageRouterConfigurationPart
  {
    public string Id { get; internal set; }

    public IReadOnlyCollection<string> QueueNamePatterns => _queueNamePatterns.AsReadOnly();

    public Type IngressPipelineConfiguratorType { get; set; }

    public void AddQueueNamePatterns([NotNull] string queueNamePattern)
    {
      if (string.IsNullOrWhiteSpace(queueNamePattern)) throw new ArgumentException(NotWhitespace, nameof(queueNamePattern));

      _queueNamePatterns.Add(queueNamePattern);
    }

    public IEnumerable<string> Validate()
    {
      if (string.IsNullOrWhiteSpace(Id)) yield return "Public API ID should be set.";
      if (!QueueNamePatterns.Any()) yield return "At least one queue name pattern should be added.";
      if (IngressPipelineConfiguratorType == null) yield return "Pipeline configurator type should be set.";
    }

    private readonly List<string> _queueNamePatterns = new List<string>();
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
