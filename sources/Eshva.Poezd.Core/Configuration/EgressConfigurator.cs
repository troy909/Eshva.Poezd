#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class EgressConfigurator : IEgressDriverConfigurator
  {
    public EgressConfigurator(BrokerEgressConfiguration configuration)
    {
      _configuration = configuration;
    }

    public EgressConfigurator WithEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public EgressConfigurator WithExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.ExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public EgressConfigurator AddPublicApi([NotNull] Action<EgressPublicApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new EgressPublicApiConfiguration();
      _configuration.AddPublicApi(configuration);
      configurator(new EgressPublicApiConfigurator(configuration));
      return this;
    }

    public void SetDriver(IEgressDriver driver)
    {
      _configuration.Driver = driver;
    }

    private readonly BrokerEgressConfiguration _configuration;
  }
}
