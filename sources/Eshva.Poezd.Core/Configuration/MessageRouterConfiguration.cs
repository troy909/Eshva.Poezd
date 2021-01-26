#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
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
      if (!_brokers.Any()) yield return "At least one message broker should be configured.";
    }

    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations()
    {
      var parts = new List<IMessageRouterConfigurationPart> {MessageHandling};
      parts.AddRange(Brokers);
      return parts;
    }

    private readonly List<MessageBrokerConfiguration> _brokers = new List<MessageBrokerConfiguration>();
  }
}
