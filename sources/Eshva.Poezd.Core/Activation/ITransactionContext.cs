#region Usings

using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface ITransactionContext : IPocket, IManageTransaction, IExecutionContextNotifier
  {
  }
}
