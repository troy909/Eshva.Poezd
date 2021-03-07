#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class IngressPublicApiConfigurator
  {
    public IngressPublicApiConfigurator([NotNull] IngressPublicApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [NotNull]
    public IngressPublicApiConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    [NotNull]
    public IngressPublicApiConfigurator WithQueueNamePatternsProvider<TQueueNamePatternsProvider>()
      where TQueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      _configuration.QueueNamePatternsProviderType = typeof(TQueueNamePatternsProvider);
      return this;
    }

    [NotNull]
    public IngressPublicApiConfigurator WithPipeFitter<TConfigurator>()
      where TConfigurator : IPipeFitter
    {
      _configuration.PipeFitterType = typeof(TConfigurator);
      return this;
    }

    [NotNull]
    public IngressPublicApiConfigurator WithHandlerRegistry<THandlerRegistryType>()
      where THandlerRegistryType : IHandlerRegistry
    {
      _configuration.HandlerRegistryType = typeof(THandlerRegistryType);
      return this;
    }

    [NotNull]
    public IngressPublicApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly IngressPublicApiConfiguration _configuration;
  }
}
