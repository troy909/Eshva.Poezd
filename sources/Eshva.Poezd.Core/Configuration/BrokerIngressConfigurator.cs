#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class BrokerIngressConfigurator : IBrokerIngressDriverConfigurator
  {
    public BrokerIngressConfigurator([NotNull] BrokerIngressConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public BrokerIngressConfigurator WithEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public BrokerIngressConfigurator WithExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.ExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public BrokerIngressConfigurator WithQueueNameMatcher<TMatcher>() where TMatcher : IQueueNameMatcher
    {
      _configuration.QueueNameMatcherType = typeof(TMatcher);
      return this;
    }

    public BrokerIngressConfigurator AddApi([NotNull] Action<IngressApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new IngressApiConfiguration();
      _configuration.AddApi(configuration);
      configurator(new IngressApiConfigurator(configuration));
      return this;
    }

    void IBrokerIngressDriverConfigurator.SetDriver(IBrokerIngressDriver driver, IMessageRouterConfigurationPart configuration)
    {
      _configuration.Driver = driver ?? throw new ArgumentNullException(nameof(driver));
      _configuration.DriverConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private readonly BrokerIngressConfiguration _configuration;
  }
}
