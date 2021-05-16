#region Usings

using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public class NonFunctionalBrokerEgress : IBrokerEgress
  {
    public BrokerEgressConfiguration Configuration => throw CreateException();

    public IBrokerEgressDriver Driver => throw CreateException();

    public IPipeFitter EnterPipeFitter => throw CreateException();

    public IPipeFitter ExitPipeFitter => throw CreateException();

    public ReadOnlyCollection<IEgressApi> Apis => throw CreateException();

    public void Dispose() { }

    public void Initialize()
    {
      throw CreateException();
    }

    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken) => throw CreateException();

    private static Exception CreateException() => new PoezdOperationException("This message broker egress doesn't function.");
  }
}
