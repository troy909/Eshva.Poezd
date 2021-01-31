#region Usings

using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Common.Collections;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public sealed class MessageHandlingContext : ConcurrentPocket, IMessageHandlingContext
  {
    public IExecutionEventSubscriptions SubscribeOn { get; } = new ExecutionEventSubscriptions();

    public Task Commit() => Task.CompletedTask;

    public Task Rollback() => Task.CompletedTask;
  }
}
