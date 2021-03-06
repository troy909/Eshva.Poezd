#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class IngressConfigurator : IIngressDriverConfigurator
  {
    public IngressConfigurator(IngressConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IngressConfigurator WithEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public IngressConfigurator WithExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.ExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public IngressConfigurator WithQueueNameMatcher<TMatcher>() where TMatcher : IQueueNameMatcher
    {
      _configuration.QueueNameMatcherType = typeof(TMatcher);
      return this;
    }

    public IngressConfigurator AddPublicApi([NotNull] Action<IngressPublicApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new IngressPublicApiConfiguration();
      _configuration.AddPublicApi(configuration);
      configurator(new IngressPublicApiConfigurator(configuration));
      return this;
    }

    void IIngressDriverConfigurator.SetDriver([NotNull] IIngressDriver driver)
    {
      _configuration.Driver = driver ?? throw new ArgumentNullException(nameof(driver));
    }

    private readonly IngressConfiguration _configuration;
  }
}
