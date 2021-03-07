#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class BrokerIngressConfiguration : CompositeMessageRouterConfigurationPart
  {
    public Type EnterPipeFitterType { get; internal set; }

    public Type ExitPipeFitterType { get; internal set; }

    public Type QueueNameMatcherType { get; internal set; }

    public IReadOnlyCollection<IngressPublicApiConfiguration> PublicApis => _publicApis.AsReadOnly();

    public IBrokerIngressDriver Driver { get; internal set; }

    public static BrokerIngressConfiguration Empty { get; } = CreateValidEmpty();

    /// <summary>
    /// Adds a public API configuration
    /// </summary>
    /// <param name="configuration">
    /// Public API configuration to add.
    /// </param>
    public void AddPublicApi([NotNull] IngressPublicApiConfiguration configuration)
    {
      if (configuration == null) throw new ArgumentNullException(nameof(configuration));

      if (_publicApis.Contains(configuration))
      {
        throw new PoezdConfigurationException(
          $"You try to add a public API {configuration.Id} which already present in the list of public APIs. It's not allowed.");
      }

      if (_publicApis.Any(api => api.Id.Equals(configuration.Id, StringComparison.InvariantCulture)))
      {
        throw new PoezdConfigurationException(
          $"A public API with ID '{configuration.Id}' already present in the list of APIs. Every API should have an unique ID.");
      }

      _publicApis.Add(configuration);
    }

    /// <inheritdoc />
    protected override IEnumerable<string> ValidateItself()
    {
      if (!_publicApis.Any())
        yield return "At least one public API should be configured for broker ingress.";
      if (QueueNameMatcherType == null)
        yield return "The queue name matcher type should be set for the broker ingress.";
      if (EnterPipeFitterType == null)
        yield return "The enter pipe fitter type should be set for the broker ingress.";
      if (ExitPipeFitterType == null)
        yield return "The exit pipe fitter type should be set for the broker ingress.";
      if (Driver == null)
        yield return "The driver should be set for the broker ingress.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() => _publicApis.AsReadOnly();

    private static BrokerIngressConfiguration CreateValidEmpty()
    {
      var configuration = new BrokerIngressConfiguration
      {
        EnterPipeFitterType = typeof(EmptyPipeFitter),
        ExitPipeFitterType = typeof(EmptyPipeFitter),
        QueueNameMatcherType = typeof(MatchingNothingQueueNameMatcher),
        Driver = new EmptyBrokerIngressDriver()
      };
      configuration.AddPublicApi(IngressPublicApiConfiguration.Empty);
      return configuration;
    }

    private readonly List<IngressPublicApiConfiguration> _publicApis = new();
  }
}
