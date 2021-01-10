#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface IExecutionEventsSubscriptions
  {
    void Committed(Func<ITransactionContext, Task> commitAction);

    void Aborted(Action<ITransactionContext> abortedAction);

    void Completed(Func<ITransactionContext, Task> completedAction);

    void Disposed(Action<ITransactionContext> disposedAction);
  }
}
