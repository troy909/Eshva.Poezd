#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// The Poezd message router. The core part of Poezd.
  /// </summary>
  public sealed partial class MessageRouter : IMessageRouter
  {
    /// <summary>
    /// Constructs a new instance of message router.
    /// </summary>
    /// <param name="configuration">
    /// The message router configuration.
    /// </param>
    /// <param name="diContainerAdapter">
    /// DI-container adapter.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    internal MessageRouter(
      [NotNull] MessageRouterConfiguration configuration,
      [NotNull] IDiContainerAdapter diContainerAdapter)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _diContainerAdapter = diContainerAdapter ?? throw new ArgumentNullException(nameof(diContainerAdapter));
      _currentState = new NotStartedState(this);
    }

    /// <inheritdoc />
    public Task Start(CancellationToken cancellationToken = default) => _currentState.Start(cancellationToken);

    /// <inheritdoc />
    public Task RouteIngressMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      object key,
      object payload,
      IReadOnlyDictionary<string, string> metadata) =>
      _currentState.RouteIngressMessage(
        brokerId,
        queueName,
        receivedOnUtc,
        key,
        payload,
        metadata);

    /// <inheritdoc />
    public Task RouteEgressMessage<TMessage>(
      TMessage message,
      string correlationId = default,
      string causationId = default,
      string messageId = default,
      DateTimeOffset timestamp = default)
      where TMessage : class =>
      _currentState.RouteEgressMessage(
        message,
        correlationId,
        causationId,
        messageId,
        timestamp);

    /// <inheritdoc />
    public void Dispose() => _currentState.Dispose();

    /// <summary>
    /// Provides the message router configuration.
    /// </summary>
    /// <param name="configurator">
    /// The message router configurator.
    /// </param>
    /// <returns>
    /// Message router configuration.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Configurator is not specified.
    /// </exception>
    public static MessageRouterConfiguration Configure([NotNull] Action<MessageRouterConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var poezdConfigurator = new MessageRouterConfigurator();
      configurator(poezdConfigurator);
      return poezdConfigurator.Configuration;
    }

    private void SetCurrentState(IMessageRouter state) => _currentState = state;

    private readonly List<IMessageBroker> _brokers = new List<IMessageBroker>();
    private readonly MessageRouterConfiguration _configuration;
    private readonly IDiContainerAdapter _diContainerAdapter;
    private IMessageRouter _currentState;
  }
}
