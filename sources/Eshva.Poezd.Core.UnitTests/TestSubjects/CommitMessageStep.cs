#region Usings

using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;
using Microsoft.Extensions.Logging;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class CommitMessageStep : IStep
  {
    public CommitMessageStep(ILogger<CommitMessageStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(IPocket context)
    {
      _logger.LogInformation("COMMIT");
      _logger.LogError("ERROR");
      _logger.LogDebug("DEBUG");
      _logger.LogTrace("TRACE");
      _logger.LogCritical("CRIT");
      _logger.LogWarning("WARNING");
      return Task.CompletedTask;
    }

    private readonly ILogger<CommitMessageStep> _logger;
  }
}
