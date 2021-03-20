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
    /// <summary>
    /// Gets the type of pipe fitter that sets the very beginning of the broker ingress pipeline up.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IPipeFitter" />.
    /// </remarks>
    public Type EnterPipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the type of pipe fitter sets the very end of the broker ingress pipeline up.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IPipeFitter" />.
    /// </remarks>
    public Type ExitPipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the type of queue name matcher.
    /// </summary>
    /// <remarks>
    /// Queue name matcher used to match the ingress message queue name to ingress API.
    /// It should implement <see cref="IQueueNameMatcher" />.
    /// </remarks>
    public Type QueueNameMatcherType { get; internal set; }

    /// <summary>
    /// Gets the list of ingress APIs.
    /// </summary>
    [NotNull]
    public IReadOnlyCollection<IngressApiConfiguration> Apis => _apis.AsReadOnly();

    /// <summary>
    /// Gets the broker ingress driver.
    /// </summary>
    public IBrokerIngressDriver Driver { get; internal set; }

    /// <summary>
    /// Gets the ingress driver configuration.
    /// </summary>
    public IMessageRouterConfigurationPart DriverConfiguration { get; internal set; }

    /// <summary>
    /// Gets an empty ingress object.
    /// </summary>
    public static BrokerIngressConfiguration Empty { get; } = CreateValidEmpty();

    /// <summary>
    /// Adds an ingress API configuration.
    /// </summary>
    /// <param name="configuration">
    /// The ingress API configuration to add.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The ingress API configuration is not specified.
    /// </exception>
    /// <exception cref="PoezdConfigurationException">
    /// The same API configuration object or an API with the same ID already present in the list.
    /// </exception>
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
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() =>
      _apis.AsReadOnly().Append(DriverConfiguration);

    private static BrokerIngressConfiguration CreateValidEmpty()
    {
      var configuration = new BrokerIngressConfiguration
      {
        Driver = new EmptyBrokerIngressDriver(),
        DriverConfiguration = new EmptyMessageRouterConfigurationPart(),
        EnterPipeFitterType = typeof(EmptyPipeFitter),
        ExitPipeFitterType = typeof(EmptyPipeFitter),
        QueueNameMatcherType = typeof(MatchingNothingQueueNameMatcher)
      };
      configuration.AddApi(IngressApiConfiguration.Empty);
      return configuration;
    }

    private readonly List<IngressApiConfiguration> _apis = new();
  }
}
