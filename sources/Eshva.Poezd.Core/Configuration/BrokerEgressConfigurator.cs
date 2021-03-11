#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class BrokerEgressConfigurator : IBrokerEgressDriverConfigurator
  {
    public BrokerEgressConfigurator(BrokerEgressConfiguration configuration)
    {
      _configuration = configuration;
    }

    public BrokerEgressConfigurator WithEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public BrokerEgressConfigurator WithExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.ExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public BrokerEgressConfigurator AddApi([NotNull] Action<EgressApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new EgressApiConfiguration();
      _configuration.AddApi(configuration);
      configurator(new EgressApiConfigurator(configuration));
      return this;
    }

    public void SetDriver(IBrokerEgressDriver driver, IMessageRouterConfigurationPart configuration)
    {
      _configuration.Driver = driver;
      _configuration.DriverConfiguration = configuration;
    }

    private readonly BrokerEgressConfiguration _configuration;
  }
}
