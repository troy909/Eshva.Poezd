#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  /// <summary>
  /// Executes all found message handlers, collects theirs commit decisions and stores them in the message handling context.
  /// </summary>
  public class ExecuteMessageHandlersStep : IStep
  {
    public ExecuteMessageHandlersStep([NotNull] IHandlersExecutionStrategy executionStrategy)
    {
      _executionStrategy = executionStrategy ?? throw new ArgumentNullException(nameof(executionStrategy));
    }

    /// <inheritdoc />
    public Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      try
      {
        var handlers = context.TakeOrThrow<IEnumerable<HandlerDescriptor>>(ContextKeys.Application.Handlers);
        var message = context.TakeOrThrow<object>(ContextKeys.Application.MessagePayload);
        var messageHandlingContext = CreateMessageHandlingContext(context);

        return _executionStrategy.ExecuteHandlers(
          handlers,
          message,
          messageHandlingContext);
      }
      catch (KeyNotFoundException exception)
      {
        throw new PoezdOperationException("A required key has not been found in the context.", exception);
      }
    }

    private static VentureContext CreateMessageHandlingContext(IPocket context)
    {
      var messageHandlingContext = new VentureContext();
      messageHandlingContext.Put(VentureContext.Keys.Message, context.TakeOrThrow<object>(ContextKeys.Application.MessagePayload));
      messageHandlingContext.Put(VentureContext.Keys.MessageType, context.TakeOrThrow<Type>(ContextKeys.Application.MessageType));
      messageHandlingContext.Put(VentureContext.Keys.MessageId, context.TakeOrNull<string>(ContextKeys.Application.MessageId));
      messageHandlingContext.Put(VentureContext.Keys.CorrelationId, context.TakeOrNull<string>(ContextKeys.Application.CorrelationId));
      messageHandlingContext.Put(VentureContext.Keys.CausationId, context.TakeOrNull<string>(ContextKeys.Application.CausationId));
      messageHandlingContext.Put(VentureContext.Keys.SourceTopic, context.TakeOrThrow<string>(ContextKeys.Broker.QueueName));

      if (context.TryTake<DateTimeOffset>(ContextKeys.Broker.ReceivedOnUtc, out var receivedOnUtc))
        messageHandlingContext.Put(VentureContext.Keys.ReceivedOnUtc, receivedOnUtc);

      return messageHandlingContext;
    }

    private readonly IHandlersExecutionStrategy _executionStrategy;
  }
}
