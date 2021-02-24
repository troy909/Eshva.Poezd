#region Usings

using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class DispatchMessageToHandlersStep : IStep
  {
    public DispatchMessageToHandlersStep(ILogger<DispatchMessageToHandlersStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(IPocket context)
    {
      _logger.LogInformation(nameof(DispatchMessageToHandlersStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<DispatchMessageToHandlersStep> _logger;
  }
}
