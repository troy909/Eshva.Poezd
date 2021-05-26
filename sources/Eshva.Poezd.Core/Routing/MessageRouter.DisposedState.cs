#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public sealed partial class MessageRouter
  {
    private class DisposedState : IMessageRouter
    {
      public void Dispose() { }

      public Task Start(CancellationToken cancellationToken = default) =>
        throw new PoezdOperationException("The message router is disposed already.");

      public Task RouteIngressMessage(
        string brokerId,
        string queueName,
        DateTimeOffset receivedOnUtc,
        object key,
        object payload,
        IReadOnlyDictionary<string, string> metadata) =>
        throw new PoezdOperationException("The message router is disposed already.");

      public Task RouteEgressMessage<TMessage>(
        TMessage message,
        string correlationId = default,
        string causationId = default,
        string messageId = default,
        DateTimeOffset timestamp = default) where TMessage : class =>
        throw new PoezdOperationException("The message router is disposed already.");
    }
  }
}
