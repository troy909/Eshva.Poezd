#region Usings

using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;
using Microsoft.Extensions.Logging;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class GetMessageHandlersStep : IStep
  {
    public GetMessageHandlersStep(ILogger<GetMessageHandlersStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(IPocket context)
    {
      _logger.LogInformation(nameof(GetMessageHandlersStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<GetMessageHandlersStep> _logger;
  }
}
