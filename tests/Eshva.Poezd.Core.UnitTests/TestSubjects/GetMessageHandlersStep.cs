#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class GetMessageHandlersStep : IStep<MessageHandlingContext>
  {
    public GetMessageHandlersStep(ILogger<GetMessageHandlersStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(MessageHandlingContext context)
    {
      _logger.LogInformation(nameof(GetMessageHandlersStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<GetMessageHandlersStep> _logger;
  }
}
