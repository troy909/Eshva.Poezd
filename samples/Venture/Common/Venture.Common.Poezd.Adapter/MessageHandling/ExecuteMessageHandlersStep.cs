#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter.MessageHandling
{
  /// <summary>
  /// Executes all found message handlers, collects theirs commit decisions and stores them in the message handling context.
  /// </summary>
  public class ExecuteMessageHandlersStep : IStep<MessageHandlingContext>
  {
    public ExecuteMessageHandlersStep([NotNull] IHandlersExecutionStrategy executionStrategy)
    {
      _executionStrategy = executionStrategy ?? throw new ArgumentNullException(nameof(executionStrategy));
    }

    /// <inheritdoc />
    public Task Execute(MessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      try
      {
        if (context.Handlers == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.Handlers));
        if (context.Message == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.Message));

        var messageHandlingContext = CreateMessageHandlingContext(context);

        return _executionStrategy.ExecuteHandlers(
          (IEnumerable<HandlerDescriptor>) context.Handlers,
          context.Message,
          messageHandlingContext);
      }
      catch (KeyNotFoundException exception)
      {
        throw new PoezdOperationException("A required key has not been found in the context.", exception);
      }
    }

    private static VentureIncomingMessageHandlingContext CreateMessageHandlingContext(MessageHandlingContext context)
    {
      if (context.Message == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.Message));
      if (context.MessageType == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.MessageType));
      if (context.ReceivedOnUtc.IsMissing()) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.ReceivedOnUtc));
      if (context.QueueName == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.QueueName));

      return new VentureIncomingMessageHandlingContext(
        context.Message,
        context.MessageType,
        context.QueueName,
        context.ReceivedOnUtc,
        context.CorrelationId,
        context.CausationId,
        context.MessageId);
    }

    private readonly IHandlersExecutionStrategy _executionStrategy;
  }
}
