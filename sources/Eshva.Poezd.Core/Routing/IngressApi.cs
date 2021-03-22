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
  /// An ingress API.
  /// </summary>
  internal class IngressApi : IIngressApi
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

    /// <inheritdoc />
    public string Id => Configuration.Id;

    /// <inheritdoc />
    public IngressApiConfiguration Configuration { get; }

    /// <inheritdoc />
    public IPipeFitter PipeFitter { get; }

    /// <inheritdoc />
    public IIngressApiMessageTypesRegistry MessageTypesRegistry { get; }

    /// <inheritdoc />
    public IHandlerRegistry HandlerRegistry { get; }

    /// <inheritdoc />
    public Type MessageKeyType => Configuration.MessageKeyType;

    /// <inheritdoc />
    public Type MessagePayloadType => Configuration.MessagePayloadType;

    /// <summary>
    /// Gets an empty ingress API.
    /// </summary>
    public static IIngressApi Empty { get; } = new EmptyIngressApi();

    /// <inheritdoc />
    public IEnumerable<string> GetQueueNamePatterns() => _queueNamePatternsProvider.GetQueueNamePatterns();

    private IPipeFitter GetIngressPipeFitter(IDiContainerAdapter serviceProvider) =>
      serviceProvider.GetService<IPipeFitter>(
        Configuration.PipeFitterType,
        exception => new PoezdConfigurationException(
          $"Can not get an instance of ingress API pipe fitter of type '{Configuration.PipeFitterType.FullName}'." +
          "You should register this type in DI-container.",
          exception));

    private IIngressApiMessageTypesRegistry GetMessageTypesRegistry(IDiContainerAdapter serviceProvider) =>
      serviceProvider.GetService<IIngressApiMessageTypesRegistry>(
        Configuration.MessageTypesRegistryType,
        exception => new PoezdConfigurationException(
          $"Can not get an instance of message types registry of type '{Configuration.MessageTypesRegistryType}'. " +
          "You should register this type in DI-container.",
          exception));

    private IHandlerRegistry GetHandlerRegistry(IDiContainerAdapter serviceProvider) =>
      serviceProvider.GetService<IHandlerRegistry>(
        Configuration.HandlerRegistryType,
        exception => new PoezdConfigurationException(
          $"Can not get an instance of handler registry of type '{Configuration.HandlerRegistryType}'. " +
          "You should register this type in DI-container.",
          exception));

    private IQueueNamePatternsProvider GetQueueNamePatternsProvider(IDiContainerAdapter serviceProvider) =>
      serviceProvider.GetService<IQueueNamePatternsProvider>(
        Configuration.QueueNamePatternsProviderType,
        exception => new PoezdConfigurationException(
          $"Can not get an instance of queue name patterns provider of type '{Configuration.QueueNamePatternsProviderType}'. " +
          "You should register this type in DI-container.",
          exception));

    private readonly IQueueNamePatternsProvider _queueNamePatternsProvider;
  }
}
