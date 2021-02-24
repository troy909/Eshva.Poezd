#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Public API.
  /// </summary>
  public sealed class PublicApi : IPublicApi
  {
    /// <summary>
    /// Constructs a new instance of public API.
    /// </summary>
    /// <param name="configuration">
    /// The public API configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public PublicApi([NotNull] PublicApiConfiguration configuration, [NotNull] IServiceProvider serviceProvider)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      _queueNamePatternsProvider = GetQueueNamePatternsProvider(serviceProvider);
      MessageTypesRegistry = GetMessageTypesRegistry(serviceProvider);
      HandlerRegistry = GetHandlerRegistry(serviceProvider);
      IngressPipeFitter = GetIngressPipeFitter(serviceProvider);
    }

    /// <inheritdoc />
    public string Id => Configuration.Id;

    /// <inheritdoc />
    public PublicApiConfiguration Configuration { get; }

    /// <inheritdoc />
    public IPipeFitter IngressPipeFitter { get; }

    /// <inheritdoc />
    public IMessageTypesRegistry MessageTypesRegistry { get; }

    /// <inheritdoc />
    public IHandlerRegistry HandlerRegistry { get; }

    /// <summary>
    /// A stab public API.
    /// </summary>
    [NotNull]
    public static IPublicApi Empty { get; } = new EmptyPublicApi();

    /// <inheritdoc />
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

    private IHandlerRegistry GetHandlerRegistry(IServiceProvider serviceProvider)
    {
      var registry = (IHandlerRegistry) serviceProvider.GetService(
        Configuration.HandlerRegistryType,
        type => new PoezdOperationException(
          $"Can not get an instance of handler registry of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
      return registry;
    }

    private readonly IQueueNamePatternsProvider _queueNamePatternsProvider;
  }
}
