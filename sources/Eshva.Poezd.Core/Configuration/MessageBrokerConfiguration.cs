#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageBrokerConfiguration
  {
    public string Id { get; internal set; }

    public IReadOnlyList<PublicApiConfiguration> PublicApis => _publicApis;

    public Type QueueNameMatcherType { get; internal set; }

    public Type PipelineConfiguratorType { get; set; }

    public void AddPublicApi([NotNull] PublicApiConfiguration publicApiConfiguration)
    {
      if (publicApiConfiguration == null) throw new ArgumentNullException(nameof(publicApiConfiguration));

      _publicApis.Add(publicApiConfiguration);
    }

    private readonly List<PublicApiConfiguration> _publicApis = new List<PublicApiConfiguration>();
  }
}
