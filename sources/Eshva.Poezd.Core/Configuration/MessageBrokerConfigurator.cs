#region Usings

using System;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageBrokerConfigurator
  {
    public MessageBrokerConfigurator([NotNull] MessageBrokerConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public MessageBrokerConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(id));

      _configuration.Id = id;
      return this;
    }

    public MessageBrokerConfigurator AddPublicApi([NotNull] Action<PublicApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var publicApiConfiguration = new PublicApiConfiguration();
      _configuration.AddPublicApi(publicApiConfiguration);
      configurator(new PublicApiConfigurator(publicApiConfiguration));
      return this;
    }

    public MessageBrokerConfigurator WithIngressEnterPipelineConfigurator<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.IngressEnterPipelineConfiguratorType = typeof(TConfigurator);
      return this;
    }

    public MessageBrokerConfigurator WithIngressExitPipelineConfigurator<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.IngressExitPipelineConfiguratorType = typeof(TConfigurator);
      return this;
    }

    public MessageBrokerConfigurator WithQueueNameMatcher<TMatcher>() where TMatcher : IQueueNameMatcher
    {
      _configuration.QueueNameMatcherType = typeof(TMatcher);
      return this;
    }

    public MessageBrokerConfigurator WithDriver<TDriverFactory, TConfigurator, TConfiguration>(Action<TConfigurator> configurator)
      where TDriverFactory : IMessageBrokerDriverFactory
    {
      _configuration.DriverFactoryType = typeof(TDriverFactory);

      var configuration =
        Activator.CreateInstance(typeof(TConfiguration)) ??
        throw new PoezdConfigurationException($"Can not create a driver configuration instance of type {typeof(TConfiguration).FullName}");
      var driverConfigurator = 
        (TConfigurator) Activator.CreateInstance(typeof(TConfigurator), configuration) ??
        throw new PoezdConfigurationException($"Can not create a driver configurator instance of type {typeof(TConfigurator).FullName}");
      _configuration.DriverConfiguration = configuration;

      configurator(driverConfigurator);
      return this;
    }

    private readonly MessageBrokerConfiguration _configuration;
  }
}
