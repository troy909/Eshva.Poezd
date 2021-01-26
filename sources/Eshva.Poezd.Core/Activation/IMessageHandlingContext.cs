#region Usings

using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface IMessageHandlingContext : IPocket, IManageTransaction, IExecutionContextNotifier { }
}
