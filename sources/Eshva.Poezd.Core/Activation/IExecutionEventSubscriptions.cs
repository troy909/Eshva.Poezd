#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface IExecutionEventSubscriptions
  {
    void Committed(Func<IMessageHandlingContext, Task> commitAction);

    void Completed(Func<IMessageHandlingContext, Task> completedAction);

    void Aborted(Action<IMessageHandlingContext> abortedAction);

    void Disposed(Action<IMessageHandlingContext> disposedAction);
  }
}
