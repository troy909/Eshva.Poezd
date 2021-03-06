#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Public API configurator.
  /// </summary>
  /// TODO: Forbid to call any With... method more than once for all configurators.
  public sealed class PublicApiConfigurator
  {
    /// <summary>
    /// Creates a new instance of public API configurator.
    /// </summary>
    /// <param name="configuration">
    /// Public API configuration object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Configuration object is not specified.
    /// </exception>
    public PublicApiConfigurator([NotNull] PublicApiConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets ID of the public API.
    /// </summary>
    /// <remarks>
    /// This ID used mainly for logging purposes.
    /// </remarks>
    /// <param name="id">
    /// The public API ID to be set.
    /// </param>
    /// <returns>
    /// This configurator object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// ID is null, an empty or whitespace string.
    /// </exception>
    [NotNull]
    public PublicApiConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    /// <summary>
    /// Sets the ingress pipe fitter type.
    /// </summary>
    /// <remarks>
    /// The Poezd router uses a fitter of this type to add steps into ingress pipeline generated for each incoming message.
    /// Steps will be added in between message broker enter and exit steps.
    /// </remarks>
    /// <typeparam name="TConfigurator">
    /// The ingress pipe fitter type.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public PublicApiConfigurator WithIngressPipeFitter<TConfigurator>()
      where TConfigurator : IPipeFitter
    {
      _configuration.IngressPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public PublicApiConfigurator WithEgressPipeFitter<TConfigurator>()
    {
      _configuration.EgressPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Sets handler registry type.
    /// </summary>
    /// <remarks>
    /// The Poezd router uses registry of this type to instantiate message handlers for incoming messages.
    /// </remarks>
    /// <typeparam name="THandlerRegistryType">
    /// The handler registry type.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public PublicApiConfigurator WithHandlerRegistry<THandlerRegistryType>()
      where THandlerRegistryType : IHandlerRegistry
    {
      _configuration.HandlerRegistryType = typeof(THandlerRegistryType);
      return this;
    }

    /// <summary>
    /// Sets queue name patterns provider type.
    /// </summary>
    /// <remarks>
    /// Queue name patterns provider providers queue/topic name patterns related to this public API that message broker will
    /// subscribe to. The format of this patterns are related to message broker driver type.
    /// </remarks>
    /// <typeparam name="TQueueNamePatternsProvider">
    /// The queue name patterns provider.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public PublicApiConfigurator WithQueueNamePatternsProvider<TQueueNamePatternsProvider>()
      where TQueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      _configuration.QueueNamePatternsProviderType = typeof(TQueueNamePatternsProvider);
      return this;
    }

    /// <summary>
    /// Sets message types registry type.
    /// </summary>
    /// <remarks>
    /// Message types registry mainly used to serialize and parse message broker messages into CLR application messages.
    /// </remarks>
    /// <typeparam name="TMessageTypesRegistry">
    /// The message types registry type.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public PublicApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly PublicApiConfiguration _configuration;
  }
}
