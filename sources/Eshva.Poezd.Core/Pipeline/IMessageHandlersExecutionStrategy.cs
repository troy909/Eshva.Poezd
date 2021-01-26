#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.MessageHandling;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public interface IMessageHandlersExecutionStrategy
  {
    Task Execute(
      IEnumerable<IHandleMessage> handlers,
      object message,
      IPocket context);
  }
}
