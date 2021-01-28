#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageBrokerDriver : IDisposable
  {
    public void Initialize(
      [NotNull] IMessageRouter messageRouter,
      [NotNull] string brokerId,
      [NotNull] object configuration);

    Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default);

    Task Publish(byte[] brokerPayload, IReadOnlyDictionary<string, string> brokerMetadata);
  }
}
