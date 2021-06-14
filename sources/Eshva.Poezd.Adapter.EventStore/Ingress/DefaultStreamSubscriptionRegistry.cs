#region Usings

using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.EventStore.Ingress
{
  public class DefaultStreamSubscriptionRegistry : IStreamSubscriptionRegistry
  {
    public void Add(IIngressApi api, IStreamSubscription streamSubscription)
    {
      // TODO: Store subscription in a list.
    }

    public void Dispose()
    {
      // TODO: Dispose or close subscriptions.
    }
  }
}
