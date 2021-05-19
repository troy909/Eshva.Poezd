#region Usings

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestBrokerEgressDriver : IBrokerEgressDriver
  {
    public TestBrokerEgressDriver(TestDriverState state)
    {
      _state = state;
    }

    public void Dispose()
    {
      _state.DisposedCount++;
    }

    public void Initialize(
      string brokerId,
      IEnumerable<IEgressApi> apis,
      IDiContainerAdapter serviceProvider)
    {
      _state.InitializedCount++;
    }

    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken)
    {
      _state.PublishingContext = context;
      _state.PublishedMessageCount++;
      return Task.CompletedTask;
    }

    private readonly TestDriverState _state;
  }
}
