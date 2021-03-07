#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  /*
  internal sealed class TestBrokerDriver : IMessageBrokerDriver
  {
    public IEnumerable<string> SubscribedQueueNamePatters { get; private set; } = new List<string>();

    public void Initialize(
      IMessageRouter messageRouter,
      string brokerId,
      object configuration)
    {
      _messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
      _configuration = (TestBrokerDriverConfiguration) (configuration ?? throw new ArgumentNullException(nameof(configuration)));
    }

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
    {
      if (_isMessageConsumingStarted)
      {
        throw new InvalidOperationException(
          $"{nameof(TestBrokerDriver)} is started already. You can subscribe to queues only before driver is started.");
      }

      SubscribedQueueNamePatters = queueNamePatterns;

      // In a real world driver you will do something to connect to the broker.
      _isMessageConsumingStarted = true;
      return Task.CompletedTask;
    }

    public Task Publish(
      byte[] key,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames) =>
      throw new NotImplementedException();

    public Task Publish(byte[] payload, IReadOnlyDictionary<string, string> metadata) => throw new NotImplementedException();

    public void Dispose() { }

    // ReSharper disable once NotAccessedField.Local In a real world driver configuration will be used in StartConsumeMessages().
    private TestBrokerDriverConfiguration _configuration;

    private bool _isMessageConsumingStarted;

    // ReSharper disable once NotAccessedField.Local It a real world driver will be used in StartConsumeMessages().
    private IMessageRouter _messageRouter;
  }
  */

  /*
  internal sealed class TestBrokerDriverFactory : IMessageBrokerDriverFactory
  {
    public IMessageBrokerDriver Create() => new TestBrokerDriver();
  }
*/
}
