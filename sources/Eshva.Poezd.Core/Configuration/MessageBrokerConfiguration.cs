#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageBrokerConfiguration : CompositeMessageRouterConfigurationPart
  {
    public string Id { get; internal set; }

    public IReadOnlyCollection<PublicApiConfiguration> PublicApis => _publicApis.AsReadOnly();

    public Type QueueNameMatcherType { get; internal set; }

    public Type IngressEnterPipelineConfiguratorType { get; set; }

    public Type IngressExitPipelineConfiguratorType { get; set; }

    public Type DriverFactoryType { get; set; }

    public object DriverConfiguration { get; set; }

    public void AddPublicApi([NotNull] PublicApiConfiguration publicApiConfiguration)
    {
      if (publicApiConfiguration == null) throw new ArgumentNullException(nameof(publicApiConfiguration));

      _publicApis.Add(publicApiConfiguration);
    }

    protected override IEnumerable<string> ValidateItself()
    {
      if (string.IsNullOrWhiteSpace(Id)) yield return "ID of message broker should be specified.";
      if (!_publicApis.Any()) yield return $"At least one public API should be configured for message broker with ID '{Id}'.";
      if (QueueNameMatcherType == null) yield return $"Queue name matcher type should be set for message broker with ID '{Id}'.";
      if (IngressEnterPipelineConfiguratorType == null)
        yield return $"Ingress enter pipeline configurator type should be set for message broker with ID '{Id}'.";
      if (IngressExitPipelineConfiguratorType== null)
        yield return $"Ingress exit pipeline configurator type should be set for message broker with ID '{Id}'.";
      if (DriverFactoryType == null) yield return $"Message broker driver factory type should be set for message broker with ID '{Id}'.";
    }

    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations()
    {
      // TODO: Add validation to PublicApiConfiguration and other configuration types.
      yield break;
    }

    private readonly List<PublicApiConfiguration> _publicApis = new List<PublicApiConfiguration>();
  }
}
