#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class TestBrokerEgressExitStep : IStep<MessagePublishingContext>
  {
    public TestBrokerEgressExitStep(ILogger<TestBrokerEgressExitStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(MessagePublishingContext context)
    {
      _logger.LogInformation(nameof(TestBrokerEgressExitStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<TestBrokerEgressExitStep> _logger;
  }
}
