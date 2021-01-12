#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Activation;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageRouter
  {
    Task RouteIncomingMessage([NotNull] object message, [NotNull] ITransactionContext transactionContext);
  }
}
