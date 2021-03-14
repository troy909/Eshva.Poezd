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
    /// <param name="configuration">
    /// The message broker configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <param name="clock"></param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public MessageBroker(
      [NotNull] MessageBrokerConfiguration configuration,
      [NotNull] IServiceProvider serviceProvider,
      [NotNull] IClock clock)
    {
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      Ingress = new BrokerIngress(configuration.Ingress, serviceProvider);
      Egress = new BrokerEgress(
        configuration.Egress,
        serviceProvider,
        clock);
    }

    /// <summary>
    /// Gets the message broker ID.
    /// </summary>
    [NotNull]
    public string Id => Configuration.Id;

    [NotNull]
    public IBrokerIngress Ingress { get; }

    [NotNull]
    public IBrokerEgress Egress { get; }

    /// <summary>
    /// The message broker configuration.
    /// </summary>
    [NotNull]
    public MessageBrokerConfiguration Configuration { get; }

    /// <inheritdoc />
    public void Dispose()
    {
      Ingress.Dispose();
      Egress.Dispose();
    }

    public void Initialize(IMessageRouter messageRouter, string brokerId)
    {
      Ingress.Initialize(messageRouter, brokerId);
      Egress.Initialize(messageRouter, brokerId);
    }

    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken) =>
      Egress.Publish(context, cancellationToken);

    public Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
    {
      if (queueNamePatterns == null) throw new ArgumentNullException(nameof(queueNamePatterns));

      return Ingress.StartConsumeMessages(queueNamePatterns, cancellationToken);
    }
  }
}
