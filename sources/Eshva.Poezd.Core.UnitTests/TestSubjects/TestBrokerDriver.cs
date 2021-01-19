#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  internal sealed class TestBrokerDriver : IMessageBrokerDriver
  {
    public TestBrokerDriver(
      [NotNull] IMessageRouter messageRouter,
      [NotNull] TestBrokerDriverConfiguration configuration)
    {
      _messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IEnumerable<string> SubscribedQueueNamePatters { get; private set; } = new List<string>();

    public Task StartConsumeMessages()
    {
      if (_isMessageConsumingStarted)
      {
        throw new InvalidOperationException($"{nameof(TestBrokerDriver)} is started already. You can start driver only once.");
      }

      // In a real world driver you will do something to connect to the broker.
      _isMessageConsumingStarted = true;
      return Task.CompletedTask;
    }

    public Task Publish(byte[] brokerPayload, IReadOnlyDictionary<string, string> brokerMetadata)
    {
      throw new NotImplementedException();
    }

    public Task SubscribeToQueues(IEnumerable<string> queueNamePatterns)
    {
      if (_isMessageConsumingStarted)
      {
        throw new InvalidOperationException(
          $"{nameof(TestBrokerDriver)} is started already. You can subscribe to queues only before driver is started.");
      }

      SubscribedQueueNamePatters = queueNamePatterns;
      return Task.CompletedTask;
    }

    public Task PublishMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] brokerPayload,
      IReadOnlyDictionary<string, string> brokerMetadata) =>
      _messageRouter.RouteIncomingMessage(
        brokerId,
        queueName,
        receivedOnUtc,
        brokerPayload,
        brokerMetadata);

    private bool _isMessageConsumingStarted;
    private readonly TestBrokerDriverConfiguration _configuration;
    private readonly IMessageRouter _messageRouter;
  }
}
