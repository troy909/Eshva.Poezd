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
    public MessageBrokerConfigurator Egress([NotNull] Action<BrokerEgressConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      _configuration.Egress = new BrokerEgressConfiguration();
      configurator(new BrokerEgressConfigurator(_configuration.Egress));
      return this;
    }

    private readonly MessageBrokerConfiguration _configuration;
  }
}
