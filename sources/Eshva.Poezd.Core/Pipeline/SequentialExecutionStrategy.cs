#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.MessageHandling;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public class SequentialExecutionStrategy : IMessageHandlersExecutionStrategy
  {
    public async Task Execute(IEnumerable<IHandleMessage> handlers, object message, IPocket context)
    {
      foreach (var handler in handlers)
      {
        await handler.Handle(message, context);
      }
    }
  }
}
