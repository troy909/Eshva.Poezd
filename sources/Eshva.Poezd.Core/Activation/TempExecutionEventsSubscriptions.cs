using System;
using System.Threading.Tasks;


namespace Eshva.Poezd.Core.Activation
{
  public class TempExecutionEventsSubscriptions : IExecutionEventsSubscriptions
  {
    public void Committed(Func<ITransactionContext, Task> commitAction)
    {
    }

    public void Aborted(Action<ITransactionContext> abortedAction)
    {
    }

    public void Completed(Func<ITransactionContext, Task> completedAction)
    {
    }

    public void Disposed(Action<ITransactionContext> disposedAction)
    {
    }
  }
}