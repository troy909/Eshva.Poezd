#region Usings

using System;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Broker egress configurator.
  /// </summary>
  public class BrokerEgressConfigurator : IBrokerEgressDriverConfigurator
  {
    /// <summary>
    /// Constructs a new instance of broker egress configurator.
    /// </summary>
    /// <param name="configuration">
    /// The egress configuration to configure with this configurator.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The egress configuration is not specified.
    /// </exception>
    public BrokerEgressConfigurator([NotNull] BrokerEgressConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets the type of the egress pipe fitter that set the very beginning of egress pipeline up.
    /// </summary>
    /// <typeparam name="TConfigurator">
    /// The type of the egress pipe fitter. It should implement <see cref="IPipeFitter" />.
    /// </typeparam>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public BrokerEgressConfigurator WithEnterPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      if (_configuration.EnterPipeFitterType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "enter pipe fitter",
          "broker egress",
          nameof(WithEnterPipeFitter));
      }

      _configuration.EnterPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Sets the type of the egress pipe fitter that sets the very end of egress pipeline up.
    /// </summary>
    /// <typeparam name="TConfigurator">
    /// The type of the egress pipe fitter. It should implement <see cref="IPipeFitter" />.
    /// </typeparam>
    /// <returns>
    /// This configurator.
    /// </returns>
    [NotNull]
    public BrokerEgressConfigurator WithExitPipeFitter<TConfigurator>() where TConfigurator : IPipeFitter
    {
      if (_configuration.ExitPipeFitterType != null)
      {
        throw ConfiguratorTools.MakeConfigurationMethodCalledMoreThanOnceException(
          "exit pipe fitter",
          "broker egress",
          nameof(WithExitPipeFitter));
      }

      _configuration.ExitPipeFitterType = typeof(TConfigurator);
      return this;
    }

    /// <summary>
    /// Adds and configures an egress API.
    /// </summary>
    /// <param name="configurator">
    /// The egress API configurator of API to add.
    /// </param>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The egress API configurator is not specified.
    /// </exception>
    [NotNull]
    public BrokerEgressConfigurator AddApi([NotNull] Action<EgressApiConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var configuration = new EgressApiConfiguration();
      _configuration.AddApi(configuration);
      configurator(new EgressApiConfigurator(configuration));
      return this;
    }

    /// <inheritdoc />
    void IBrokerEgressDriverConfigurator.SetDriver(IBrokerEgressDriver driver, IMessageRouterConfigurationPart configuration)
    {
      _configuration.Driver = driver ?? throw new ArgumentNullException(nameof(driver));
      _configuration.DriverConfiguration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    private readonly BrokerEgressConfiguration _configuration;
  }
}
