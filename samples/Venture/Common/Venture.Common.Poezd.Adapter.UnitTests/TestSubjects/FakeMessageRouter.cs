#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.UnitTests.TestSubjects
{
  public class FakeMessageRouter : IMessageRouter
  {
    // ReSharper disable once NotNullMemberIsNotInitialized
    // ReSharper disable once UnassignedGetOnlyAutoProperty
    public IReadOnlyCollection<MessageBroker> Brokers { get; }

    public object Message { get; private set; }

    public string CorrelationId { get; private set; }

    public string CausationId { get; private set; }

    public string MessageId { get; private set; }

    public Task Start(CancellationToken cancellationToken = default) => throw new InvalidOperationException();

    public Task RouteIncomingMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata) =>
      throw new InvalidOperationException();

    public Task RouteOutgoingMessage<TMessage>(
      TMessage message,
      string correlationId = default,
      string causationId = default,
      string messageId = default)
      where TMessage : class
    {
      Message = message;
      CorrelationId = correlationId;
      CausationId = causationId;
      MessageId = messageId;
      return Task.CompletedTask;
    }
  }
}
