#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageRouterConfiguration : CompositeMessageRouterConfigurationPart
  {
    public MessageHandlingConfiguration MessageHandling { get; } = new MessageHandlingConfiguration();

    public IReadOnlyCollection<MessageBrokerConfiguration> Brokers => _brokers.AsReadOnly();

    internal void AddBroker([NotNull] MessageBrokerConfiguration brokerConfiguration) =>
      _brokers.Add(brokerConfiguration ?? throw new ArgumentNullException(nameof(brokerConfiguration)));

    protected override IEnumerable<string> ValidateItself()
    {
      // TODO: Implement
      yield break;
    }

    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations()
    {
      // TODO: Implement
      yield break;
    }

    private readonly List<MessageBrokerConfiguration> _brokers = new List<MessageBrokerConfiguration>();
  }
}
