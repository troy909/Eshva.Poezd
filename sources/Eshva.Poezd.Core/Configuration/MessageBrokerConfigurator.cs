#region Usings

using System;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// A message broker configurator.
  /// </summary>
  // TODO: Disallow call methods more than once.
  public sealed class MessageBrokerConfigurator
  {
    /// <summary>
    /// Creates an instance of a message broker configurator.
    /// </summary>
    /// <param name="configuration">
    /// The message broker configuration to configure with this configurator.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// The message broker configuration is not specified.
    /// </exception>
    public MessageBrokerConfigurator([NotNull] MessageBrokerConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Sets the ID of the message broker.
    /// </summary>
    /// <remarks>
    /// This ID used mainly for logging purposes.
    /// </remarks>
    /// <param name="id">
    /// The message broker ID to be set.
    /// </param>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The ID is null, an empty or a whitespace string.
    /// </exception>
    [NotNull]
    public MessageBrokerConfigurator WithId([NotNull] string id)
    {
      if (string.IsNullOrWhiteSpace(id)) throw new ArgumentNullException(nameof(id));

      _configuration.Id = id;
      return this;
    }

    /// <summary>
    /// Configures the message broker ingress.
    /// </summary>
    /// <param name="configurator">
    /// The the message broker ingress configurator.
    /// </param>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The the message broker ingress configurator is not specified.
    /// </exception>
    /// <exception cref="PoezdConfigurationException">
    /// Ingress() and WithoutIngress() called for the same broker.
    /// </exception>
    [NotNull]
    public MessageBrokerConfigurator Ingress([NotNull] Action<BrokerIngressConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));
      EnsureIngressIsNotConfiguredYet();

      _configuration.Ingress = new BrokerIngressConfiguration();
      configurator(new BrokerIngressConfigurator(_configuration.Ingress));
      _isIngressConfiguredAlready = true;
      return this;
    }

    /// <summary>
    /// Configures broker with no ingress message handling.
    /// </summary>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="PoezdConfigurationException">
    /// Ingress() and WithoutIngress() called for the same broker.
    /// </exception>
    public MessageBrokerConfigurator WithoutIngress()
    {
      EnsureIngressIsNotConfiguredYet();

      _configuration.Ingress = new BrokerIngressConfiguration();
      _configuration.HasNoIngress = true;
      _isIngressConfiguredAlready = true;
      return this;
    }

    /// <summary>
    /// Configures the message broker egress.
    /// </summary>
    /// <param name="configurator">
    /// The the message broker egress configurator.
    /// </param>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The the message broker egress configurator is not specified.
    /// </exception>
    /// <exception cref="PoezdConfigurationException">
    /// Egress() and WithoutEgress() called for the same broker.
    /// </exception>
    [NotNull]
    public MessageBrokerConfigurator Egress([NotNull] Action<BrokerEgressConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));
      EnsureEgressIsNotConfiguredYet();

      _configuration.Egress = new BrokerEgressConfiguration();
      configurator(new BrokerEgressConfigurator(_configuration.Egress));
      _isEgressConfiguredAlready = true;
      return this;
    }

    /// <summary>
    /// Configures broker with no egress message handling.
    /// </summary>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="PoezdConfigurationException">
    /// Egress() and WithoutEgress() called for the same broker.
    /// </exception>
    public MessageBrokerConfigurator WithoutEgress()
    {
      EnsureEgressIsNotConfiguredYet();

      _configuration.Egress = new BrokerEgressConfiguration();
      _configuration.HasNoEgress = true;
      _isEgressConfiguredAlready = true;
      return this;
    }

    private void EnsureIngressIsNotConfiguredYet()
    {
      if (_isIngressConfiguredAlready)
      {
        throw new PoezdConfigurationException(
          $"It's not allowed to call {nameof(Ingress)}() and {nameof(WithoutIngress)}() for the same broker.");
      }
    }

    private void EnsureEgressIsNotConfiguredYet()
    {
      if (_isEgressConfiguredAlready)
      {
        throw new PoezdConfigurationException(
          $"It's not allowed to call {nameof(Egress)}() and {nameof(WithoutEgress)}() for the same broker.");
      }
    }

    private readonly MessageBrokerConfiguration _configuration;
    private bool _isEgressConfiguredAlready;
    private bool _isIngressConfiguredAlready;
  }
}
