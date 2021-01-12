#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface ITransactionContext : IPocket, IManageTransaction, IExecutionContextNotifier
  {
  }

  public class TransactionContext : PocketExecutionContext, ITransactionContext
  {
    public Task Commit()
    {
      return Task.CompletedTask;
    }

    public Task Rollback()
    {
      return Task.CompletedTask;
    }

    public IExecutionEventsSubscriptions SubscribeOn { get; } = new TempExecutionEventsSubscriptions();
  }

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
