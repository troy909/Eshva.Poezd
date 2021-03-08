#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
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

      _apis.Add(configuration);
    }

    protected override IEnumerable<string> ValidateItself()
    {
      if (!_apis.Any())
        yield return "At least one API should be configured for broker egress.";
      if (EnterPipeFitterType == null)
        yield return "The enter pipe fitter type should be set for the broker egress.";
      if (ExitPipeFitterType == null)
        yield return "The exit pipe fitter type should be set for the broker egress.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() => _apis.AsReadOnly();

    private static BrokerEgressConfiguration CreateValidEmpty()
    {
      var configuration = new BrokerEgressConfiguration
      {
        Driver = new EmptyBrokerEgressDriver(),
        EnterPipeFitterType = typeof(EmptyPipeFitter),
        ExitPipeFitterType = typeof(EmptyPipeFitter)
      };
      configuration.AddApi(EgressApiConfiguration.Empty);
      return configuration;
    }

    private readonly List<EgressApiConfiguration> _apis = new();
  }
}
