#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestBrokerEgressEnterStep : IStep<MessagePublishingContext>
  {
    public TestBrokerEgressEnterStep(ILogger<TestBrokerEgressEnterStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(MessagePublishingContext context)
    {
      _logger.LogInformation(nameof(TestBrokerEgressEnterStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<TestBrokerEgressEnterStep> _logger;
  }
}
