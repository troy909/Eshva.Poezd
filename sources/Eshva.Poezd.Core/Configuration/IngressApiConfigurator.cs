#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public class IngressApiConfigurator
  {
    public IngressApiConfigurator([NotNull] IngressApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    [NotNull]
    public IngressApiConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    [NotNull]
    public IngressApiConfigurator WithMessageKey<TMessageKey>()
    {
      _configuration.MessageKeyType = typeof(TMessageKey);
      return this;
    }

    [NotNull]
    public IngressApiConfigurator WithMessagePayload<TMessagePayload>()
    {
      _configuration.MessagePayloadType = typeof(TMessagePayload);
      return this;
    }

    [NotNull]
    public IngressApiConfigurator WithQueueNamePatternsProvider<TQueueNamePatternsProvider>()
      where TQueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      _configuration.QueueNamePatternsProviderType = typeof(TQueueNamePatternsProvider);
      return this;
    }

    [NotNull]
    public IngressApiConfigurator WithPipeFitter<TConfigurator>()
      where TConfigurator : IPipeFitter
    {
      _configuration.PipeFitterType = typeof(TConfigurator);
      return this;
    }

    [NotNull]
    public IngressApiConfigurator WithHandlerRegistry<THandlerRegistryType>()
      where THandlerRegistryType : IHandlerRegistry
    {
      _configuration.HandlerRegistryType = typeof(THandlerRegistryType);
      return this;
    }

    [NotNull]
    public IngressApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
      where TMessageTypesRegistry : IIngressMessageTypesRegistry
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly IngressApiConfiguration _configuration;
  }
}
