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
  public sealed class PublicApi : IPublicApi
  {
    public PublicApi([NotNull] PublicApiConfiguration configuration, [NotNull] IServiceProvider serviceProvider)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      _queueNamePatternsProvider = GetQueueNamePatternsProvider(serviceProvider);
      MessageTypesRegistry = GetMessageTypesRegistry(serviceProvider);
      IngressPipeFitter = GetIngressPipeFitter(serviceProvider);
    }

    public string Id => Configuration.Id;

    public PublicApiConfiguration Configuration { get; }

    public IPipeFitter IngressPipeFitter { get; }

    public IMessageTypesRegistry MessageTypesRegistry { get; }

    [NotNull]
    public static IPublicApi Empty { get; } = new EmptyPublicApi();

    public IEnumerable<string> GetQueueNamePatterns() => _queueNamePatternsProvider.GetQueueNamePatterns();

    private IPipeFitter GetIngressPipeFitter(IServiceProvider serviceProvider)
    {
      var pipeFitter = (IPipeFitter) serviceProvider.GetService(
        Configuration.IngressPipeFitterType,
        type => new PoezdOperationException(
          $"Can not get an instance of public API ingress pipeline configurator of type '{type.FullName}'." +
          "You should register this type in DI-container."));
      return pipeFitter;
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

    private IMessageTypesRegistry GetMessageTypesRegistry(IServiceProvider serviceProvider)
    {
      var registry = (IMessageTypesRegistry) serviceProvider.GetService(
        Configuration.MessageTypesRegistryType,
        type => new PoezdOperationException(
          $"Can not get an instance of message types registry of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
      return registry;
    }

    private readonly IQueueNamePatternsProvider _queueNamePatternsProvider;

    private class EmptyPublicApi : IPublicApi
    {
      public string Id => typeof(EmptyPublicApi).FullName!;

      public PublicApiConfiguration Configuration => new PublicApiConfiguration();

      public IPipeFitter IngressPipeFitter { get; } = new EmptyPipeFitter();

      public IMessageTypesRegistry MessageTypesRegistry { get; } = new EmptyMessageTypesRegistry();

      public IEnumerable<string> GetQueueNamePatterns() => Enumerable.Empty<string>();
    }
  }
}
