#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

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
      ILogger<IBrokerEgressDriver> logger,
      IClock clock,
      IEnumerable<IEgressApi> apis,
      IServiceProvider serviceProvider)
    {
      _state.InitializedCount++;
    }

    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken)
    {
      _state.PublishedMessageCount++;
      return Task.CompletedTask;
    }

    private readonly TestDriverState _state;
  }
}
