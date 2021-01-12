using System.Threading.Tasks;
using Eshva.Common;


namespace Eshva.Poezd.Core.Activation
{
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
}