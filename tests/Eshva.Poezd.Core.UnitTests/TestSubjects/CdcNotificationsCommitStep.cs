#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class CdcNotificationsCommitStep : IStep<MessageHandlingContext>
  {
    public CdcNotificationsCommitStep(ILogger<CdcNotificationsCommitStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(MessageHandlingContext context)
    {
      _logger.LogInformation(nameof(CdcNotificationsCommitStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<CdcNotificationsCommitStep> _logger;
  }
}
