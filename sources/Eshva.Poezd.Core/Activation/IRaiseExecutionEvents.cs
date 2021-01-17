#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface IRaiseExecutionEvents
  {
    Task RaiseCommittedEvent(IMessageHandlingContext context);

    Task RaiseCompletedEvent(IMessageHandlingContext context);

    void RaiseAbortedEvent(IMessageHandlingContext context);

    void RaiseDisposedEvent(IMessageHandlingContext context);
  }
}
