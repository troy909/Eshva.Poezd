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
      _logger.LogInformation(nameof(CommitMessageStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<CommitMessageStep> _logger;
  }
}
