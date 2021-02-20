#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Message router configurator.
  /// </summary>
  public sealed class MessageRouterConfigurator
  {
    /// <summary>
    /// Gets the message router configuration object handled by this configurator.
    /// </summary>
    [NotNull]
    public MessageRouterConfiguration Configuration { get; } = new MessageRouterConfiguration();

    /// <summary>
    /// Adds a message broker handled by configuring message router.
    /// </summary>
    /// <param name="configurator">
    /// Message broker configurator.
    /// </param>
    /// <returns>
    /// This configurator object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The configurator is not specified.
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
