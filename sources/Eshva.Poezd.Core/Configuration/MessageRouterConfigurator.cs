#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Poezd message router configurator.
  /// </summary>
  public sealed class MessageRouterConfigurator
  {
    /// <summary>
    /// Gets the message router configuration used by this configurator.
    /// </summary>
    [NotNull]
    public MessageRouterConfiguration Configuration { get; } = new();

    /// <summary>
    /// Adds a message broker to message router.
    /// </summary>
    /// <param name="configurator">
    /// Message broker configurator.
    /// </param>
    /// <returns>
    /// This configurator.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message broker configurator is not specified.
    /// </exception>
    [NotNull]
    public MessageRouterConfigurator AddMessageBroker([NotNull] Action<MessageBrokerConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var brokerConfiguration = new MessageBrokerConfiguration();
      Configuration.AddBroker(brokerConfiguration);
      configurator(new MessageBrokerConfigurator(brokerConfiguration));
      return this;
    }
  }
}
