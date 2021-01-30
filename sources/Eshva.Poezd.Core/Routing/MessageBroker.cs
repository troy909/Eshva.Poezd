#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public sealed class MessageBroker : IDisposable
  {
    public MessageBroker(
      [NotNull] IMessageBrokerDriver driver,
      [NotNull] MessageBrokerConfiguration configuration,
      [NotNull] IServiceProvider serviceProvider)
    {
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      Driver = driver ?? throw new ArgumentNullException(nameof(driver));
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      PublicApis = configuration.PublicApis.Select(
        apiConfiguration => new PublicApi(apiConfiguration, serviceProvider)).ToList().AsReadOnly();
      _queueNameMatcher = (IQueueNameMatcher) serviceProvider.GetService(configuration.QueueNameMatcherType);
      IngressEnterPipelineConfigurator = GetIngressEnterPipelineConfigurator(serviceProvider);
      IngressExitPipelineConfigurator = GetIngressExitPipelineConfigurator(serviceProvider);
    }

    public IMessageBrokerDriver Driver { get; }

    public string Id => Configuration.Id;

    public IReadOnlyCollection<IPublicApi> PublicApis { get; }

    public object DriverConfiguration => Configuration.DriverConfiguration;

    public MessageBrokerConfiguration Configuration { get; }

    public IPipelineConfigurator IngressEnterPipelineConfigurator { get; set; }

    public IPipelineConfigurator IngressExitPipelineConfigurator { get; set; }

    public void Dispose()
    {
      Driver?.Dispose();
    }

    public IPublicApi GetApiByQueueName(string queueName)
    {
      var publicApi = PublicApis.FirstOrDefault(
        api => api.GetQueueNamePatterns()
          .Any(queueNamePattern => _queueNameMatcher.DoesMatch(queueName, queueNamePattern)));
      return publicApi ?? PublicApi.Empty;
    }

    private IPipelineConfigurator GetIngressEnterPipelineConfigurator(IServiceProvider serviceProvider)
    {
      return (IPipelineConfigurator) serviceProvider.GetService(
        Configuration.IngressEnterPipelineConfiguratorType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker ingress enter pipeline configurator of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private IPipelineConfigurator GetIngressExitPipelineConfigurator(IServiceProvider serviceProvider)
    {
      return (IPipelineConfigurator) serviceProvider.GetService(
        Configuration.IngressExitPipelineConfiguratorType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker ingress exit pipeline configurator of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private readonly IQueueNameMatcher _queueNameMatcher;
  }
}
