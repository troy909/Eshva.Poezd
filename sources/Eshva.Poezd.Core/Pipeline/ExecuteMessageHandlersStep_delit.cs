#region Usings

using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.MessageHandling;
using Eshva.Poezd.Core.Routing;

#endregion


namespace Eshva.Poezd.Core.Pipeline
{
  public sealed class ExecuteMessageHandlersStep_delit : IStep
  {
    public ExecuteMessageHandlersStep_delit(IMessageHandlersExecutionStrategy messageHandlersExecutionStrategy)
    {
      _messageHandlersExecutionStrategy = messageHandlersExecutionStrategy;
    }

    public Task Execute(IPocket context)
    {
      var handlers = context.GetContextVariable<IEnumerable<IHandleMessage>>(ContextKeys.MessageHandling.Handlers);
      var message = context.GetContextVariable<object>(ContextKeys.Application.MessagePayload);

      return _messageHandlersExecutionStrategy.Execute(
        handlers,
        message,
        context);
    }

    private readonly IMessageHandlersExecutionStrategy _messageHandlersExecutionStrategy;
  }
}
