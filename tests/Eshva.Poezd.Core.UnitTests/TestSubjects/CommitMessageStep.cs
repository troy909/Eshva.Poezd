#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class CommitMessageStep : IStep<MessageHandlingContext>
  {
    public CommitMessageStep(ILogger<CommitMessageStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(MessageHandlingContext context)
    {
      _logger.LogInformation(nameof(CommitMessageStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<CommitMessageStep> _logger;
  }
}
