using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

namespace Eshva.Poezd.Core.Configuration
{
  public class IngressPublicApiConfigurator
  {
    public IngressPublicApiConfigurator(IngressPublicApiConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IngressPublicApiConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    public IngressPublicApiConfigurator WithQueueNamePatternsProvider<TQueueNamePatternsProvider>()
      where TQueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      _configuration.QueueNamePatternsProviderType = typeof(TQueueNamePatternsProvider);
      return this;
    }

    public IngressPublicApiConfigurator WithPipeFitter<TConfigurator>()
      where TConfigurator : IPipeFitter
    {
      _configuration.IngressPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public IngressPublicApiConfigurator WithHandlerRegistry<THandlerRegistryType>()
      where THandlerRegistryType : IHandlerRegistry
    {
      _configuration.HandlerRegistryType = typeof(THandlerRegistryType);
      return this;
    }

    public IngressPublicApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly IngressPublicApiConfiguration _configuration;
  }
}
