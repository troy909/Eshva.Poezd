#region Usings

using System;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.EventStore.Ingress
{
  public interface IStreamSubscriptionRegistry : IDisposable
  {
    void Add(IIngressApi api, IStreamSubscription streamSubscription);

    // TODO: May be add a Get method?
  }
}
