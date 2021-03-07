#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Message broker configurator.
  /// </summary>
  public sealed class MessageBrokerConfigurator
  {
    /// <summary>
    /// Creates an instance of message broker configurator.
    /// </summary>
    /// <param name="configuration">
    /// Message broker configuration object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Configuration object is not specified.
    /// </exception>
    public MessageBrokerConfigurator([NotNull] MessageBrokerConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets ID of the message broker.
    /// </summary>
    /// <remarks>
    /// This ID used mainly for logging purposes.
    /// </remarks>
    /// <param name="id">
    /// The message broker ID to be set.
    /// </param>
    /// <returns>
    /// This configurator object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// ID is null, an empty or whitespace string.
    /// </exception>
    [NotNull]
    public MessageBrokerConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    [NotNull]
    public MessageBrokerConfigurator Ingress([NotNull] Action<BrokerIngressConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      _configuration.Ingress = new BrokerIngressConfiguration();
      configurator(new BrokerIngressConfigurator(_configuration.Ingress));
      return this;
    }

    [NotNull]
    public MessageBrokerConfigurator Egress([NotNull] Action<EgressConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      _configuration.Egress = new BrokerEgressConfiguration();
      configurator(new EgressConfigurator(_configuration.Egress));
      return this;
    }

    private readonly MessageBrokerConfiguration _configuration;
  }

  /*
  /// <summary>
  /// Message broker configurator.
  /// </summary>
  public sealed class MessageBrokerConfigurator
  {
    /// <summary>
    /// Creates an instance of message broker configurator.
    /// </summary>
    /// <param name="configuration">
    /// Message broker configuration object.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Configuration object is not specified.
    /// </exception>
    public MessageBrokerConfigurator([NotNull] MessageBrokerConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets ID of the message broker.
    /// </summary>
    /// <remarks>
    /// This ID used mainly for logging purposes.
    /// </remarks>
    /// <param name="id">
    /// The message broker ID to be set.
    /// </param>
    /// <returns>
    /// This configurator object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// ID is null, an empty or whitespace string.
    /// </exception>
    [NotNull]
    public MessageBrokerConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    /// <summary>
    /// Adds a public API hosted by this message broker.
    /// </summary>
    /// <param name="configurator">
    /// Configurator of the public API to be added.
    /// </param>
    /// <returns>
    /// This configurator object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The configurator is not specified.
    /// </exception>
    [NotNull]
    public MessageBrokerConfigurator AddPublicApi([NotNull] Action<PublicApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var publicApiConfiguration = new PublicApiConfiguration();
      _configuration.AddPublicApi(publicApiConfiguration);
      configurator(new PublicApiConfigurator(publicApiConfiguration));
      return this;
    }

    /// <summary>
    /// Sets the ingress enter pipe fitter type.
    /// </summary>
    /// <remarks>
    /// The Poezd router uses a fitter of this type to add steps into ingress pipeline generated for each incoming message.
    /// Steps will be added in the very beginning of pipeline in front of public API related steps.
    /// </remarks>
    /// <typeparam name="TConfigurator">
    /// The ingress enter pipe fitter type.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public MessageBrokerConfigurator WithIngressEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.IngressEnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Sets the ingress exit pipe fitter type.
    /// </summary>
    /// <remarks>
    /// The Poezd router uses a fitter of this type to add steps into ingress pipeline generated for each incoming message.
    /// Steps will be added in the very end of pipeline in just after of public API related steps.
    /// </remarks>
    /// <typeparam name="TConfigurator">
    /// The ingress exit pipe fitter type.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public MessageBrokerConfigurator WithIngressExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.IngressExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Sets the egress enter pipe fitter type.
    /// </summary>
    /// <remarks>
    /// The Poezd router uses a fitter of this type to add steps into egress pipeline generated for each outgoing message.
    /// Steps will be added in the very beginning of pipeline in front of public API related steps.
    /// </remarks>
    /// <typeparam name="TConfigurator">
    /// The egress enter pipe fitter type.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public MessageBrokerConfigurator WithEgressEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EgressEnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Sets the egress exit pipe fitter type.
    /// </summary>
    /// <remarks>
    /// The Poezd router uses a fitter of this type to add steps into egress pipeline generated for each outgoing message.
    /// Steps will be added in the very end of pipeline in just after of public API related steps.
    /// </remarks>
    /// <typeparam name="TConfigurator">
    /// The egress exit pipe fitter type.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public MessageBrokerConfigurator WithEgressExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EgressExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Sets the queue name matcher type.
    /// </summary>
    /// <remarks>
    /// The Poezd router uses a queue name matcher of this type to test queue/topic name of incoming messages.
    /// TODO: Explain how it is used.
    /// </remarks>
    /// <typeparam name="TMatcher">
    /// The queue name matcher type.
    /// </typeparam>
    /// <returns>
    /// This configurator object.
    /// </returns>
    [NotNull]
    public MessageBrokerConfigurator WithQueueNameMatcher<TMatcher>() where TMatcher : IQueueNameMatcher
    {
      _configuration.QueueNameMatcherType = typeof(TMatcher);
      return this;
    }

    /// <summary>
    /// Sets message broker driver related types.
    /// </summary>
    /// <typeparam name="TDriverFactory">
    /// Driver factory type.
    /// </typeparam>
    /// <typeparam name="TConfigurator">
    /// Driver configurator type.
    /// </typeparam>
    /// <typeparam name="TConfiguration">
    /// Driver configuration type.
    /// </typeparam>
    /// <param name="configurator">
    /// Configurator of message broker driver.
    /// </param>
    /// <returns>
    /// This configurator object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The configurator is not specified.
    /// </exception>
    /// TODO: Try to leave only TDriverFactory type parameter.
    [NotNull]
    public MessageBrokerConfigurator WithDriver<TDriverFactory, TConfigurator, TConfiguration>(Action<TConfigurator> configurator)
      where TDriverFactory : IMessageBrokerDriverFactory
    {
      _configuration.DriverFactoryType = typeof(TDriverFactory);

      var configuration =
        Activator.CreateInstance(typeof(TConfiguration)) ??
        throw new PoezdConfigurationException($"Can not create a driver configuration instance of type {typeof(TConfiguration).FullName}");
      var driverConfigurator =
        (TConfigurator) Activator.CreateInstance(typeof(TConfigurator), configuration) ??
        throw new PoezdConfigurationException($"Can not create a driver configurator instance of type {typeof(TConfigurator).FullName}");
      _configuration.DriverConfiguration = configuration;

      configurator(driverConfigurator);
      return this;
    }

    public MessageBrokerConfigurator Ingress(Action<IngressConfigurator> configurator)
    {
      _configuration.Ingress = new IngressConfiguration();
      configurator(new IngressConfigurator(_configuration.Ingress));
      return this;
    }

    public MessageBrokerConfigurator Egress(Action<EgressConfigurator> configurator)
    {
      _configuration.Egress = new EgressConfiguration();
      configurator(new EgressConfigurator(_configuration.Egress));
      return this;
    }

    private readonly MessageBrokerConfiguration _configuration;
  }
*/
}
