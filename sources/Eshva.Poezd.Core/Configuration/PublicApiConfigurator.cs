#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  public sealed class PublicApiConfigurator
  {
    public PublicApiConfigurator([NotNull] PublicApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public PublicApiConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentException(NotWhitespace, nameof(id));

      _configuration.Id = id;
      return this;
    }

    public PublicApiConfigurator WithIngressPipelineConfigurator<TConfigurator>()
      where TConfigurator : IPipeFitter
    {
      _configuration.IngressPipelineConfiguratorType = typeof(TConfigurator);
      return this;
    }

    public PublicApiConfigurator WithHandlerRegistry<THandlerRegistryType>()
      where THandlerRegistryType : IHandlerRegistry
    {
      _configuration.HandlerRegistryType = typeof(THandlerRegistryType);
      return this;
    }

    public PublicApiConfigurator WithQueueNamePatternsProvider<TQueueNamePatternsProvider>()
      where TQueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      _configuration.QueueNamePatternsProviderType = typeof(TQueueNamePatternsProvider);
      return this;
    }

    public PublicApiConfigurator WithMessageTypesRegistry<TMessageRegistry>()
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageRegistry);
      return this;
    }

    private readonly PublicApiConfiguration _configuration;
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
