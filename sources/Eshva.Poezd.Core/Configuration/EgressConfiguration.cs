#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class EgressConfiguration : CompositeMessageRouterConfigurationPart
  {
    public Type EnterPipeFitterType { get; internal set; }

    public Type ExitPipeFitterType { get; internal set; }

    public IReadOnlyCollection<EgressPublicApiConfiguration> PublicApis => _publicApis.AsReadOnly();

    public IEgressDriver Driver { get; internal set; }

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
        yield return "At least one public API should be configured for broker ingress.";
      if (EnterPipeFitterType == null)
        yield return "The enter pipe fitter type should be set for the broker ingress.";
      if (ExitPipeFitterType == null)
        yield return "The exit pipe fitter type should be set for the broker ingress.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() => _publicApis.AsReadOnly();

    private readonly List<EgressPublicApiConfiguration> _publicApis = new();
  }
}
