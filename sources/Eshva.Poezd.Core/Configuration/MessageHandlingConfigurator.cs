#region Usings

using System;
using Eshva.Poezd.Core.Activation;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageHandlingConfigurator
  {
    public MessageHandlingConfigurator([NotNull] MessageHandlingConfiguration messageHandlingConfiguration)
    {
      _messageHandlingConfiguration = messageHandlingConfiguration ?? throw new ArgumentNullException(nameof(messageHandlingConfiguration));
    }

    public MessageHandlingConfigurator WithMessageHandlersFactory(IMessageHandlersFactory messageHandlersFactory)
    {
      _messageHandlingConfiguration.MessageHandlersFactory =
        messageHandlersFactory ?? throw new ArgumentNullException(nameof(messageHandlersFactory));
      return this;
    }

    private readonly MessageHandlingConfiguration _messageHandlingConfiguration;
  }
}
