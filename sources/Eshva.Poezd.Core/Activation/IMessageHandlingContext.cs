#region Usings

using Eshva.Common;
using Eshva.Common.Collections;

#endregion


namespace Eshva.Poezd.Core.Activation
{
  public interface IMessageHandlingContext : IPocket, IManageTransaction, IExecutionContextNotifier { }
}
