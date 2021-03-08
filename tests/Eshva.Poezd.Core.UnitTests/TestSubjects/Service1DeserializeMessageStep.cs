#region Usings

using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class Service1DeserializeMessageStep : IStep<MessageHandlingContext>
  {
    public Service1DeserializeMessageStep(ILogger<Service1DeserializeMessageStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(MessageHandlingContext context)
    {
      _logger.LogInformation(nameof(Service1DeserializeMessageStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<Service1DeserializeMessageStep> _logger;
  }
}
