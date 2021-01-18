#region Usings

using System;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class PoezdConfigurator
  {
    public MessageRouterConfiguration Configuration { get; } = new MessageRouterConfiguration();

    public PoezdConfigurator WithMessageHandling([NotNull] Action<MessageHandlingConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      configurator(new MessageHandlingConfigurator(Configuration.MessageHandling));
      return this;
    }

    public PoezdConfigurator AddMessageBroker([NotNull] Action<MessageBrokerConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var brokerConfiguration = new MessageBrokerConfiguration();
      Configuration.AddBroker(brokerConfiguration);
      configurator(new MessageBrokerConfigurator(brokerConfiguration));
      return this;
    }
  }
}
