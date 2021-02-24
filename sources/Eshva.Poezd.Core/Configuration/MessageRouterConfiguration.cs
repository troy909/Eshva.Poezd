#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Configuration of Poezd message router.
  /// </summary>
  public sealed class MessageRouterConfiguration : CompositeMessageRouterConfigurationPart
  {
    /// <summary>
    /// The list of message brokers managed by this instance of message router.
    /// </summary>
    public IReadOnlyCollection<MessageBrokerConfiguration> Brokers => _brokers.AsReadOnly();

    /// <summary>
    /// Adds a message broker configuration.
    /// </summary>
    /// <param name="configuration">
    /// A message broker configuration to add.
    /// </param>
    internal void AddBroker([NotNull] MessageBrokerConfiguration configuration) =>
      _brokers.Add(configuration ?? throw new ArgumentNullException(nameof(configuration)));

    /// <inheritdoc />
    protected override IEnumerable<string> ValidateItself()
    {
      if (!_brokers.Any()) yield return "At least one message broker should be configured.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations() => Brokers;

    private readonly List<MessageBrokerConfiguration> _brokers = new();
  }
}
