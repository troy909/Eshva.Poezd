#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Configuration
{
  /// <summary>
  /// Configuration of a message broker.
  /// </summary>
  public sealed class MessageBrokerConfiguration : CompositeMessageRouterConfigurationPart
  {
    /// <summary>
    /// Gets ID of the message broker.
    /// </summary>
    [NotNull]
    public string Id { get; internal set; } = string.Empty;

    [NotNull]
    public BrokerIngressConfiguration Ingress
    {
      get => _ingress;
      internal set => _ingress = value ?? throw new ArgumentNullException(nameof(value));
    }

    [NotNull]
    public BrokerEgressConfiguration Egress
    {
      get => _egress;
      internal set => _egress = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc />
    protected override IEnumerable<string> ValidateItself()
    {
      if (string.IsNullOrWhiteSpace(Id)) yield return "ID of the message broker should be specified.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations()
    {
      yield return Ingress;
      yield return Egress;
    }

    [NotNull]
    private BrokerEgressConfiguration _egress = BrokerEgressConfiguration.Empty;

    [NotNull]
    private BrokerIngressConfiguration _ingress = BrokerIngressConfiguration.Empty;
  }
}
