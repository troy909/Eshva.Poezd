#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class PublicApiConfiguration
  {
    public string Id { get; internal set; }

    public IReadOnlyCollection<string> QueueNamePatterns => _queueNamePatterns.AsReadOnly();

    public string Name { get; internal set; }

    public Type AdapterType { get; internal set; }

    public Type PipelineConfiguratorType { get; set; }

    public Type QueueNameMatcherType { get; set; }

    public void AddQueueNamePatterns([NotNull] string queueNamePattern)
    {
      if (string.IsNullOrWhiteSpace(queueNamePattern)) throw new ArgumentException(NotWhitespace, nameof(queueNamePattern));

      _queueNamePatterns.Add(queueNamePattern);
    }

    private readonly List<string> _queueNamePatterns = new List<string>();
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
