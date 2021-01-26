#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.MessageHandling;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public class ParallelExecutionStrategy : IMessageHandlersExecutionStrategy
  {
    public Task Execute(
      IEnumerable<IHandleMessage> handlers,
      object message,
      IPocket context)
    {
      var handlingTasks = handlers.Select(handler => handler.Handle(message, context)).ToList();
      return Task.WhenAll(handlingTasks);
    }
  }
}
