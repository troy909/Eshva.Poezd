#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;
using Eshva.Poezd.Core.Pipeline;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public sealed class PublicApi : IPublicApi
  {
    public PublicApi([NotNull] PublicApiConfiguration configuration, [NotNull] IServiceProvider serviceProvider)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      _queueNamePatternsProvider = GetQueueNamePatternsProvider(serviceProvider);
      IngressPipeFitter = GetIngressPipelineConfigurator(serviceProvider);
    }

    public string Id => Configuration.Id;

    public PublicApiConfiguration Configuration { get; }

    public static IPublicApi Empty { get; } = new EmptyPublicApi();

    public IPipeFitter IngressPipeFitter { get; }

    public IEnumerable<string> GetQueueNamePatterns() => _queueNamePatternsProvider.GetQueueNamePatterns();

    private IPipeFitter GetIngressPipelineConfigurator(IServiceProvider serviceProvider)
    {
      var configurator = (IPipeFitter) serviceProvider.GetService(
        Configuration.IngressPipelineConfiguratorType,
        type => new PoezdOperationException(
          $"Can not get an instance of public API ingress pipeline configurator of type '{type.FullName}'." +
          "You should register this type in DI-container."));
      return configurator;
    }

    private IQueueNamePatternsProvider GetQueueNamePatternsProvider(IServiceProvider serviceProvider)
    {
      var provider = (IQueueNamePatternsProvider) serviceProvider.GetService(
        Configuration.QueueNamePatternsProviderType,
        type => new PoezdOperationException(
          $"Can not get an instance of queue name patterns provider of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
      return provider;
    }

    private readonly IQueueNamePatternsProvider _queueNamePatternsProvider;

    private class EmptyPublicApi : IPublicApi
    {
      public string Id => typeof(EmptyPublicApi).FullName;

      public PublicApiConfiguration Configuration => new PublicApiConfiguration();

      public IPipeFitter IngressPipeFitter { get; } = new EmptyPipeFitter();

      public IEnumerable<string> GetQueueNamePatterns() => Enumerable.Empty<string>();
    }
  }
}
