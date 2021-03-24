#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// Contract of API producer encapsulating Kafka API consumer.
  /// </summary>
  public interface IApiProducer : IDisposable
  {
    /// <summary>
    /// Publishes a message from publishing <paramref name="context"/>.
    /// </summary>
    /// <param name="context">
    /// The publishing context containing message.
    /// </param>
    /// <param name="cancellationToken">
    /// The cancellation token.
    /// </param>
    /// <returns>
    /// Task that can be used to wait the message published is finished.
    /// </returns>
    Task Publish([NotNull] MessagePublishingContext context, CancellationToken cancellationToken);
  }
}
