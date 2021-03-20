#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Broker ingress configurator.
  /// </summary>
  public class BrokerIngressConfigurator : IBrokerIngressDriverConfigurator
  {
    /// <summary>
    /// Constructs a new instance of broker ingress configurator.
    /// </summary>
    /// <param name="configuration">
    /// The ingress configuration to configure with this configurator.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The ingress configuration is not specified.
    /// </exception>
    public BrokerIngressConfigurator([NotNull] BrokerIngressConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets the type of the ingress pipe fitter that set the very beginning of ingress pipeline up.
    /// </summary>
    /// <typeparam name="TConfigurator">
    /// The type of the ingress pipe fitter. It should implement <see cref="IPipeFitter" />.
    /// </typeparam>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public BrokerIngressConfigurator WithEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.EnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Sets the type of the ingress pipe fitter that sets the very end of ingress pipeline up.
    /// </summary>
    /// <typeparam name="TConfigurator">
    /// The type of the ingress pipe fitter. It should implement <see cref="IPipeFitter" />.
    /// </typeparam>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public BrokerIngressConfigurator WithExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      _configuration.ExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Sets the type of the queue name matcher.
    /// </summary>
    /// <remarks>
    /// Queue name matcher used to match the ingress message queue name to ingress API.
    /// </remarks>
    /// <typeparam name="TMatcher">
    /// The type of the queue name matcher.
    /// </typeparam>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public BrokerIngressConfigurator WithQueueNameMatcher<TMatcher>() where TMatcher : IQueueNameMatcher
    {
      _configuration.QueueNameMatcherType = typeof(TMatcher);
      return this;
    }

    /// <summary>
    /// Adds and configures an ingress API.
    /// </summary>
    /// <param name="configurator">
    /// The ingress API configurator of API to add.
    /// </param>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The ingress API configurator is not specified.
    /// </exception>
    [NotNull]
    public BrokerIngressConfigurator AddApi([NotNull] Action<IngressApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new IngressApiConfiguration();
      _configuration.AddApi(configuration);
      configurator(new IngressApiConfigurator(configuration));
      return this;
    }

    /// <inheritdoc />
    void IBrokerIngressDriverConfigurator.SetDriver(IBrokerIngressDriver driver, IMessageRouterConfigurationPart configuration)
    {
      _configuration.Driver = driver ?? throw new ArgumentNullException(nameof(driver));
      _configuration.DriverConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private readonly BrokerIngressConfiguration _configuration;
  }
}
