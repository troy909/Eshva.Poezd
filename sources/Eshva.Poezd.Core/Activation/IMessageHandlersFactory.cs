#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.MessageHandling;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface IMessageHandlersFactory
  {
    Task<IEnumerable<IHandleMessage>> GetHandlersOfMessage(object message, ITransactionContext transactionContext);
  }
}
