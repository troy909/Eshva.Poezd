#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageBroker : IDisposable
  {
    /// <summary>
    /// Gets the message broker ID.
    /// </summary>
    string Id { get; }

    IBrokerIngress Ingress { get; }

    IBrokerEgress Egress { get; }

    /// <summary>
    /// The message broker configuration.
    /// </summary>
    MessageBrokerConfiguration Configuration { get; }

    void Initialize(IMessageRouter messageRouter, string brokerId);

    Task Publish(MessagePublishingContext context, CancellationToken cancellationToken);

    Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default);
  }
}
