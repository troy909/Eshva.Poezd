#region Usings

using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class Service2DeserializeMessageStep : IStep<IPocket>
  {
    public Service2DeserializeMessageStep(ILogger<Service2DeserializeMessageStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(IPocket context)
    {
      _logger.LogInformation(nameof(Service2DeserializeMessageStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<Service2DeserializeMessageStep> _logger;
  }
}
