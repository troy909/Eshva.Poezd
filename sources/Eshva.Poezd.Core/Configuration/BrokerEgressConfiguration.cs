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
  /// <summary>
  /// Configuration of a broker egress.
  /// </summary>
  public class BrokerEgressConfiguration : CompositeMessageRouterConfigurationPart
  {
    /// <summary>
    /// Gets the type of pipe fitter that sets the very beginning of the broker egress pipeline up.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IPipeFitter" />.
    /// </remarks>
    public Type EnterPipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the type of pipe fitter sets the very end of the broker egress pipeline up.
    /// </summary>
    /// <remarks>
    /// It should implement <see cref="IPipeFitter" />.
    /// </remarks>
    public Type ExitPipeFitterType { get; internal set; }

    /// <summary>
    /// Gets the list of egress APIs.
    /// </summary>
    public IReadOnlyCollection<EgressApiConfiguration> Apis => _apis.AsReadOnly();

    /// <summary>
    /// Gets the broker egress driver.
    /// </summary>
    public IBrokerEgressDriver Driver { get; internal set; }

    /// <summary>
    /// Gets the egress driver configuration.
    /// </summary>
    public IMessageRouterConfigurationPart DriverConfiguration { get; internal set; }

    /// <summary>
    /// Adds an egress API configuration.
    /// </summary>
    /// <param name="configuration">
    /// The egress API configuration to add.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The egress API configuration is not specified.
    /// </exception>
    /// <exception cref="PoezdConfigurationException">
    /// The same API configuration object or an API with the same ID already present in the list.
    /// </exception>
    public void AddApi([NotNull] EgressApiConfiguration configuration)
    {
      if (configuration == null) throw new ArgumentNullException(nameof(configuration));

      if (_apis.Contains(configuration))
      {
        throw new PoezdConfigurationException(
          $"You try to add an egress API {configuration.Id} which already present in the list of APIs. It's not allowed.");
      }

      // ReSharper disable once ParameterOnlyUsedForPreconditionCheck.Local
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

    private readonly List<EgressApiConfiguration> _apis = new List<EgressApiConfiguration>();
  }
}
