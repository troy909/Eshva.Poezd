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
  public class IngressApi : IIngressApi
  {
    /// <summary>
    /// Constructs a new instance of ingress API.
    /// </summary>
    /// <param name="configuration">
    /// The ingress API configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public IngressApi([NotNull] IngressApiConfiguration configuration, [NotNull] IDiContainerAdapter serviceProvider)
    {
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      _queueNamePatternsProvider = GetQueueNamePatternsProvider(serviceProvider);
      MessageTypesRegistry = GetMessageTypesRegistry(serviceProvider);
      HandlerRegistry = GetHandlerRegistry(serviceProvider);
      PipeFitter = GetIngressPipeFitter(serviceProvider);
    }

    public string Id => Configuration.Id;

    /// <inheritdoc />
    public IngressApiConfiguration Configuration { get; }

    /// <inheritdoc />
    public IPipeFitter PipeFitter { get; }

    /// <inheritdoc />
    public IIngressMessageTypesRegistry MessageTypesRegistry { get; }

    /// <inheritdoc />
    public IHandlerRegistry HandlerRegistry { get; }

    /// <inheritdoc />
    public Type MessageKeyType => Configuration.MessageKeyType;

    /// <inheritdoc />
    public Type MessagePayloadType => Configuration.MessagePayloadType;

    public static IIngressApi Empty { get; } = new EmptyIngressApi();

    /// <inheritdoc />
    public IEnumerable<string> GetQueueNamePatterns() => _queueNamePatternsProvider.GetQueueNamePatterns();

    private IPipeFitter GetIngressPipeFitter(IDiContainerAdapter serviceProvider)
    {
      var pipeFitter = (IPipeFitter) serviceProvider.GetService(
        Configuration.PipeFitterType,
        type => new PoezdOperationException(
          $"Can not get an instance of ingress API pipe fitter of type '{type.FullName}'." +
          "You should register this type in DI-container."));
      return pipeFitter;
    }

    private IIngressMessageTypesRegistry GetMessageTypesRegistry(IDiContainerAdapter serviceProvider)
    {
      var registry = (IIngressMessageTypesRegistry) serviceProvider.GetService(
        Configuration.MessageTypesRegistryType,
        type => new PoezdOperationException(
          $"Can not get an instance of message types registry of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
      return registry;
    }

    private IHandlerRegistry GetHandlerRegistry(IDiContainerAdapter serviceProvider)
    {
      var registry = (IHandlerRegistry) serviceProvider.GetService(
        Configuration.HandlerRegistryType,
        type => new PoezdOperationException(
          $"Can not get an instance of handler registry of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
      return registry;
    }

    private IQueueNamePatternsProvider GetQueueNamePatternsProvider(IDiContainerAdapter serviceProvider)
    {
      var provider = (IQueueNamePatternsProvider) serviceProvider.GetService(
        Configuration.QueueNamePatternsProviderType,
        type => new PoezdOperationException(
          $"Can not get an instance of queue name patterns provider of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
      return provider;
    }

    private readonly IQueueNamePatternsProvider _queueNamePatternsProvider;
  }
}
