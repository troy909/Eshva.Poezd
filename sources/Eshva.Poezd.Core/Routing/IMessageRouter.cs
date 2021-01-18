#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageRouter
  {
    Task Start(CancellationToken cancellationToken = default);

    Task RouteIncomingMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] brokerPayload,
      IReadOnlyDictionary<string, string> brokerMetadata);
  }
}
