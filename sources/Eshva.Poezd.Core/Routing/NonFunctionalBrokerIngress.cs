#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  internal class NonFunctionalBrokerIngress : IBrokerIngress
  {
    public BrokerIngressConfiguration Configuration => throw CreateException();

    public IBrokerIngressDriver Driver => throw CreateException();

    public IReadOnlyCollection<IIngressApi> Apis => throw CreateException();

    public IPipeFitter EnterPipeFitter => throw CreateException();

    public IPipeFitter ExitPipeFitter => throw CreateException();

    public void Dispose() { }

    public void Initialize() => throw CreateException();

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default) =>
      throw CreateException();

    public Task RouteIngressMessage(
      string queueName,
      DateTimeOffset receivedOnUtc,
      object key,
      object payload,
      IReadOnlyDictionary<string, string> metadata) =>
      throw CreateException();

    public IIngressApi GetApiByQueueName(string queueName) => throw CreateException();

    private static Exception CreateException() => new PoezdOperationException("This message broker ingress doesn't function.");
  }
}
