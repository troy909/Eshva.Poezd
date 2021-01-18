#region Usings

using System.Threading.Tasks;
using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public sealed class MessageHandlingContext : PocketExecutionContext, IMessageHandlingContext
  {
    public IExecutionEventSubscriptions SubscribeOn { get; } = new ExecutionEventSubscriptions();

    public Task Commit()
    {
      return Task.CompletedTask;
    }

    public Task Rollback()
    {
      return Task.CompletedTask;
    }
  }
}
