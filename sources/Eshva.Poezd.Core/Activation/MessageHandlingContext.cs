using System.Threading.Tasks;
using Eshva.Common;


namespace Eshva.Poezd.Core.Activation
{
  public sealed class MessageHandlingContext : PocketExecutionContext, IMessageHandlingContext
  {
    public Task Commit()
    {
      return Task.CompletedTask;
    }

    public Task Rollback()
    {
      return Task.CompletedTask;
    }

    public IExecutionEventSubscriptions SubscribeOn { get; } = new ExecutionEventSubscriptions();
  }
}
