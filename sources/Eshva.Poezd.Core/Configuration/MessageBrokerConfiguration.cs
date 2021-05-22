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
    /// Gets the ID of the message broker.
    /// </summary>
    [NotNull]
    public string Id { get; internal set; } = string.Empty;

    /// <summary>
    /// Gets the message broker ingress.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Gets ingress but this broker doesn't handle ingress messages.
    /// </exception>
    [NotNull]
    public BrokerIngressConfiguration Ingress
    {
      get
      {
        if (HasNoIngress) throw new InvalidOperationException("This broker doesn't handle ingress messages.");
        return _ingress;
      }
      internal set => _ingress = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets the message broker egress.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// Gets ingress but this broker doesn't handle egress messages.
    /// </exception>
    [NotNull]
    public BrokerEgressConfiguration Egress
    {
      get
      {
        if (HasNoEgress) throw new InvalidOperationException("This broker doesn't handle egress messages.");
        return _egress;
      }
      internal set => _egress = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Gets indicator that this broker doesn't handle ingress messages.
    /// </summary>
    public bool HasNoIngress { get; internal set; }

    /// <summary>
    /// Gets indicator that this broker doesn't handle egress messages.
    /// </summary>
    public bool HasNoEgress { get; internal set; }

    /// <inheritdoc />
    protected override IEnumerable<string> ValidateItself()
    {
      // TODO: Extend error messages with code samples.
      if (string.IsNullOrWhiteSpace(Id)) yield return "ID of the message broker should be specified.";
    }

    /// <inheritdoc />
    protected override IEnumerable<IMessageRouterConfigurationPart> GetChildConfigurations()
    {
      if (!HasNoIngress) yield return Ingress;
      if (!HasNoEgress) yield return Egress;
    }

    private BrokerEgressConfiguration _egress;
    private BrokerIngressConfiguration _ingress;
  }
}
