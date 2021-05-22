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
  /// Message broker.
  /// </summary>
  public sealed class MessageBroker : IMessageBroker
  {
    /// <summary>
    /// Construct a new instance of message broker.
    /// </summary>
    /// <param name="messageRouter">
    /// The message router this broker belongs to.
    /// </param>
    /// <param name="configuration">
    /// The message broker configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public MessageBroker(
      [NotNull] IMessageRouter messageRouter,
      [NotNull] MessageBrokerConfiguration configuration,
      [NotNull] IDiContainerAdapter serviceProvider)
    {
      _messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      Ingress = !Configuration.HasNoIngress
        ? new BrokerIngress(
          this,
          configuration.Ingress,
          serviceProvider)
        : new NonFunctionalBrokerIngress();
      Egress = !Configuration.HasNoEgress
        ? new BrokerEgress(
          configuration.Egress,
          serviceProvider)
        : new NonFunctionalBrokerEgress();
    }

    /// <inheritdoc />
    [NotNull]
    public string Id => Configuration.Id;

    /// <inheritdoc />
    [NotNull]
    public IBrokerIngress Ingress { get; }

    /// <inheritdoc />
    [NotNull]
    public IBrokerEgress Egress { get; }

    /// <inheritdoc />
    [NotNull]
    public MessageBrokerConfiguration Configuration { get; }

    /// <inheritdoc />
    public void Dispose()
    {
      Ingress.Dispose();
      Egress.Dispose();
    }

    /// <inheritdoc />
    public void Initialize()
    {
      if (!Configuration.HasNoIngress) Ingress.Initialize();
      if (!Configuration.HasNoEgress) Egress.Initialize();
    }

    /// <inheritdoc />
    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
    {
      if (queueNamePatterns == null) throw new ArgumentNullException(nameof(queueNamePatterns));

      return Ingress.StartConsumeMessages(queueNamePatterns, cancellationToken);
    }

    /// <inheritdoc />
    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken) =>
      Egress.Publish(context, cancellationToken);

    /// <inheritdoc />
    public Task RouteIngressMessage(
      string queueName,
      DateTimeOffset receivedOnUtc,
      object key,
      object payload,
      IReadOnlyDictionary<string, string> metadata) =>
      _messageRouter.RouteIngressMessage(
        Id,
        queueName,
        receivedOnUtc,
        key,
        payload,
        metadata);

    private readonly IMessageRouter _messageRouter;
  }
}
