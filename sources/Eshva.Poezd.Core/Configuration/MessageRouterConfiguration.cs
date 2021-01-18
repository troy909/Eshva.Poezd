#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageRouterConfiguration
  {
    public MessageHandlingConfiguration MessageHandling { get; } = new MessageHandlingConfiguration();

    public IReadOnlyList<MessageBrokerConfiguration> Brokers => _brokers.AsReadOnly();

    internal void AddBroker([NotNull] MessageBrokerConfiguration brokerConfiguration) =>
      _brokers.Add(brokerConfiguration ?? throw new ArgumentNullException(nameof(brokerConfiguration)));

    private readonly List<MessageBrokerConfiguration> _brokers = new List<MessageBrokerConfiguration>();
  }
}
