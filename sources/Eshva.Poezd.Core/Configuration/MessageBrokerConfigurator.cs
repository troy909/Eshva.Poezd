#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// A message broker configurator.
  /// </summary>
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
    [NotNull]
    public MessageBrokerConfigurator Ingress([NotNull] Action<BrokerIngressConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      _configuration.Ingress = new BrokerIngressConfiguration();
      configurator(new BrokerIngressConfigurator(_configuration.Ingress));
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
    [NotNull]
    public MessageBrokerConfigurator Egress([NotNull] Action<BrokerEgressConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      _configuration.Egress = new BrokerEgressConfiguration();
      configurator(new BrokerEgressConfigurator(_configuration.Egress));
      return this;
    }

    /// <summary>
    /// Configures broker with no egress message handling.
    /// </summary>
    /// <returns>
    /// This configurator.
    /// </returns>
    public MessageBrokerConfigurator WithoutEgress()
    {
      _configuration.Egress = new BrokerEgressConfiguration();
      _configuration.HasNoEgress = true;
      return this;
    }

    /// <summary>
    /// Configures broker with no ingress message handling.
    /// </summary>
    /// <returns>
    /// This configurator.
    /// </returns>
    public MessageBrokerConfigurator WithoutIngress()
    {
      _configuration.Ingress = new BrokerIngressConfiguration();
      _configuration.HasNoIngress = true;
      return this;
    }

    private readonly MessageBrokerConfiguration _configuration;
  }
}
