#region Usings

using System;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
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
      _configuration.Ingress = new IngressConfiguration();
      configurator(new EgressConfigurator(_configuration.Egress));
      return this;
    }

    private readonly MessageBrokerConfiguration _configuration;
  }

  public class EgressConfigurator : IEgressDriverConfigurator
  {
    public EgressConfigurator(EgressConfiguration configuration)
    {
      _configuration = configuration;
    }

    public EgressConfigurator WithEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public EgressConfigurator WithExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.ExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public EgressConfigurator AddPublicApi([NotNull] Action<EgressPublicApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new EgressPublicApiConfiguration();
      _configuration.AddPublicApi(configuration);
      configurator(new EgressPublicApiConfigurator(configuration));
      return this;
    }

    IEgressDriver IEgressDriverConfigurator.Driver { get; set; }

    private readonly EgressConfiguration _configuration;
  }

  public class EgressPublicApiConfiguration
  {
    public string Id { get; set; }

    public Type PipeFitterType { get; set; }

    public Type MessageTypesRegistryType { get; set; }
  }

  public class EgressPublicApiConfigurator
  {
    public EgressPublicApiConfigurator(EgressPublicApiConfiguration configuration)
    {
      _configuration = configuration;
    }

    public EgressPublicApiConfigurator WithId(string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    public EgressPublicApiConfigurator WithPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.PipeFitterType = typeof(TConfigurator);
      return this;
    }

    public EgressPublicApiConfigurator WithMessageTypesRegistry<TMessageTypesRegistry>()
    {
      _configuration.MessageTypesRegistryType = typeof(TMessageTypesRegistry);
      return this;
    }

    private readonly EgressPublicApiConfiguration _configuration;
  }

  public class IngressConfiguration
  {
    public Type EnterPipeFitterType { get; set; }

    public Type ExitPipeFitterType { get; set; }

    public Type QueueNameMatcherType { get; set; }

    public void AddPublicApi(IngressPublicApiConfiguration configuration)
    {
      throw new NotImplementedException();
    }
  }

  public class IngressConfigurator : IIngressDriverConfigurator
  {
    public IngressConfigurator(IngressConfiguration configuration)
    {
      _configuration = configuration;
    }

    public IngressConfigurator WithEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public IngressConfigurator WithExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.ExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    public IngressConfigurator WithQueueNameMatcher<TMatcher>() where TMatcher : IQueueNameMatcher
    {
      _configuration.QueueNameMatcherType = typeof(TMatcher);
      return this;
    }

    public IngressConfigurator AddPublicApi([NotNull] Action<IngressPublicApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new IngressPublicApiConfiguration();
      _configuration.AddPublicApi(configuration);
      configurator(new IngressPublicApiConfigurator(configuration));
      return this;
    }

    IIngressDriver IIngressDriverConfigurator.Driver { get; set; }

    private readonly IngressConfiguration _configuration;
  }

  public class IngressPublicApiConfiguration
  {
    public string Id { get; set; }

    public Type QueueNamePatternsProviderType { get; set; }

    public Type IngressPipeFitterType { get; set; }

    public Type HandlerRegistryType { get; set; }

    public Type MessageTypesRegistryType { get; set; }
  }

  public class EgressConfiguration
  {
    public Type EnterPipeFitterType { get; set; }

    public Type ExitPipeFitterType { get; set; }

    public EgressConfiguration AddPublicApi(EgressPublicApiConfiguration configuration) => throw new NotImplementedException();
  }

  public interface IIngressDriverConfigurator
  {
    IIngressDriver Driver { get; set; }
  }

  public interface IEgressDriverConfigurator
  {
    IEgressDriver Driver { get; set; }
  }

  public interface IEgressDriver { }

  public interface IIngressDriver { }
}
