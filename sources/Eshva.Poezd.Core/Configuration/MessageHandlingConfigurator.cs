#region Usings

using System;
using Eshva.Poezd.Core.Activation;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public class MessageHandlingConfigurator
  {
    public MessageHandlingConfigurator([NotNull] MessageHandling messageHandling)
    {
      _messageHandling = messageHandling ?? throw new ArgumentNullException(nameof(messageHandling));
    }

    public MessageHandlingConfigurator WithMessageHandlersFactory(IMessageHandlersFactory messageHandlersFactory)
    {
      _messageHandling.MessageHandlersFactory = messageHandlersFactory ?? throw new ArgumentNullException(nameof(messageHandlersFactory));
      return this;
    }

    private readonly MessageHandling _messageHandling;
  }
}
