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

    public IReadOnlyCollection<IngressApiConfiguration> Apis => _apis.AsReadOnly();

    public IBrokerIngressDriver Driver { get; internal set; }

    public static BrokerIngressConfiguration Empty { get; } = CreateValidEmpty();

    /// <summary>
    /// Adds an ingress API configuration
    /// </summary>
    /// <param name="configuration">
    /// Ingress API configuration to add.
    /// </param>
    public void AddApi([NotNull] IngressApiConfiguration configuration)
    {
      if (configuration == null) throw new ArgumentNullException(nameof(configuration));

      if (_apis.Contains(configuration))
      {
        throw new PoezdConfigurationException(
          $"You try to add an ingress API {configuration.Id} which already present in the list of APIs. It's not allowed.");
      }

      if (_apis.Any(api => api.Id.Equals(configuration.Id, StringComparison.InvariantCulture)))
      {
        throw new PoezdConfigurationException(
          $"An ingress API with ID '{configuration.Id}' already present in the list of APIs. Every API should have an unique ID.");
      }

      _apis.Add(configuration);
    }

    /// <inheritdoc />
    protected override IEnumerable<string> ValidateItself()
    {
      if (!_apis.Any())
        yield return "At least one API should be configured for broker ingress.";
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
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() => _apis.AsReadOnly();

    private static BrokerIngressConfiguration CreateValidEmpty()
    {
      var configuration = new BrokerIngressConfiguration
      {
        EnterPipeFitterType = typeof(EmptyPipeFitter),
        ExitPipeFitterType = typeof(EmptyPipeFitter),
        QueueNameMatcherType = typeof(MatchingNothingQueueNameMatcher),
        Driver = new EmptyBrokerIngressDriver()
      };
      configuration.AddApi(IngressApiConfiguration.Empty);
      return configuration;
    }

    private readonly List<IngressApiConfiguration> _apis = new();
  }
}
