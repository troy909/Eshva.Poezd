#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestBrokerIngressDriver : IBrokerIngressDriver
  {
    public IEnumerable<string> SubscribedQueueNamePatters { get; private set; } = Enumerable.Empty<string>();

    public void Dispose()
    {
      throw new NotImplementedException();
    }

    public void Initialize(
      IMessageRouter messageRouter,
      string brokerId,
      IServiceProvider serviceProvider)
    {
      _messageRouter = messageRouter;
    }

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
    {
      if (_isMessageConsumingStarted)
      {
        throw new InvalidOperationException(
          $"{nameof(TestBrokerIngressDriver)} is started already. You can subscribe to queues only before driver is started.");
      }

      SubscribedQueueNamePatters = queueNamePatterns;

      // In a real world driver you will do something to connect to the broker.
      _isMessageConsumingStarted = true;
      return Task.CompletedTask;
    }

    private bool _isMessageConsumingStarted;

    // ReSharper disable once NotAccessedField.Local It a real world driver will be used in StartConsumeMessages().
    private IMessageRouter _messageRouter;
  }
}
