#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class DispatchMessageToHandlersStep : IStep<MessageHandlingContext>
  {
    public DispatchMessageToHandlersStep(ILogger<DispatchMessageToHandlersStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(MessageHandlingContext context)
    {
      _logger.LogInformation(nameof(DispatchMessageToHandlersStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<DispatchMessageToHandlersStep> _logger;
  }
}
