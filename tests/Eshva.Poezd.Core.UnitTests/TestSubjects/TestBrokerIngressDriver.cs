#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestBrokerIngressDriver : IBrokerIngressDriver
  {
    public TestBrokerIngressDriver(TestDriverState state)
    {
      _state = state;
    }

    public void Dispose()
    {
      _state.DisposedCount++;
    }

    public void Initialize(
      string brokerId,
      IMessageRouter messageRouter,
      IEnumerable<IIngressApi> apis,
      IServiceProvider serviceProvider)
    {
      _state.InitializedCount++;
    }

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
    {
      if (_isMessageConsumingStarted)
      {
        throw new InvalidOperationException(
          $"{nameof(TestBrokerIngressDriver)} is started already. You can subscribe to queues only before driver is started.");
      }

      _state.SubscribedQueueNamePatters.AddRange(queueNamePatterns);
      _state.MessageConsumingStartedCount++;

      // In a real world driver you will do something to connect to the broker.
      _isMessageConsumingStarted = true;
      return Task.CompletedTask;
    }

    private readonly TestDriverState _state;
    private bool _isMessageConsumingStarted;
  }
}
