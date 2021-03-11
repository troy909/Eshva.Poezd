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
  public class BrokerEgressConfiguration : CompositeMessageRouterConfigurationPart
  {
    public Type EnterPipeFitterType { get; internal set; }

    public Type ExitPipeFitterType { get; internal set; }

    public IReadOnlyCollection<EgressApiConfiguration> Apis => _apis.AsReadOnly();

    public IBrokerEgressDriver Driver { get; internal set; }

    public IMessageRouterConfigurationPart DriverConfiguration { get; internal set; }

    public static BrokerEgressConfiguration Empty { get; } = CreateValidEmpty();

    /// <summary>
    /// Adds an egress API configuration
    /// </summary>
    /// <param name="configuration">
    /// Egress API configuration to add.
    /// </param>
    public void AddApi([NotNull] EgressApiConfiguration configuration)
    {
      if (configuration == null) throw new ArgumentNullException(nameof(configuration));

      if (_apis.Contains(configuration))
      {
        throw new PoezdConfigurationException(
          $"You try to add an egress API {configuration.Id} which already present in the list of APIs. It's not allowed.");
      }

      if (_apis.Any(api => api.Id.Equals(configuration.Id, StringComparison.InvariantCulture)))
      {
        throw new PoezdConfigurationException(
          $"An egress API with ID '{configuration.Id}' already present in the list of APIs. Every API should have an unique ID.");
      }

      _apis.Add(configuration);
    }

    /// <inheritdoc />
    protected override IEnumerable<string> ValidateItself()
    {
      if (!_apis.Any())
        yield return "At least one API should be configured for broker egress.";
      if (EnterPipeFitterType == null)
        yield return "The enter pipe fitter type should be set for the broker egress.";
      if (ExitPipeFitterType == null)
        yield return "The exit pipe fitter type should be set for the broker egress.";
      if (Driver == null)
        yield return "The driver should be set for the broker egress.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() =>
      _apis.AsReadOnly().Append(DriverConfiguration);

    private static BrokerEgressConfiguration CreateValidEmpty()
    {
      var configuration = new BrokerEgressConfiguration
      {
        Driver = new EmptyBrokerEgressDriver(),
        DriverConfiguration = new EmptyBrokerEgressDriverConfiguration(),
        EnterPipeFitterType = typeof(EmptyPipeFitter),
        ExitPipeFitterType = typeof(EmptyPipeFitter)
      };
      configuration.AddApi(EgressApiConfiguration.Empty);
      return configuration;
    }

    private readonly List<EgressApiConfiguration> _apis = new();
  }
}
