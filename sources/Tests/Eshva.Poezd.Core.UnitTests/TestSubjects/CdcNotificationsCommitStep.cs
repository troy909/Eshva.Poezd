#region Usings

using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class CdcNotificationsCommitStep : IStep
  {
    public CdcNotificationsCommitStep(ILogger<CdcNotificationsCommitStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(IPocket context)
    {
      _logger.LogInformation(nameof(CdcNotificationsCommitStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<CdcNotificationsCommitStep> _logger;
  }
}
