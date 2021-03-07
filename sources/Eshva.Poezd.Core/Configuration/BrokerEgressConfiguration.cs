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

    public IReadOnlyCollection<EgressPublicApiConfiguration> PublicApis => _publicApis.AsReadOnly();

    public IEgressDriver Driver { get; internal set; }

    public static BrokerEgressConfiguration Empty { get; } = CreateValidEmpty();

    /// <summary>
    /// Adds a public API configuration
    /// </summary>
    /// <param name="configuration">
    /// Public API configuration to add.
    /// </param>
    public void AddPublicApi([NotNull] EgressPublicApiConfiguration configuration)
    {
      if (configuration == null) throw new ArgumentNullException(nameof(configuration));

      _publicApis.Add(configuration);
    }

    protected override IEnumerable<string> ValidateItself()
    {
      if (!_publicApis.Any())
        yield return "At least one public API should be configured for broker egress.";
      if (EnterPipeFitterType == null)
        yield return "The enter pipe fitter type should be set for the broker egress.";
      if (ExitPipeFitterType == null)
        yield return "The exit pipe fitter type should be set for the broker egress.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() => _publicApis.AsReadOnly();

    private static BrokerEgressConfiguration CreateValidEmpty()
    {
      var configuration = new BrokerEgressConfiguration
      {
        Driver = new EmptyEgressDriver(),
        EnterPipeFitterType = typeof(EmptyPipeFitter),
        ExitPipeFitterType = typeof(EmptyPipeFitter)
      };
      configuration.AddPublicApi(EgressPublicApiConfiguration.Empty);
      return configuration;
    }

    private readonly List<EgressPublicApiConfiguration> _publicApis = new();
  }
}
