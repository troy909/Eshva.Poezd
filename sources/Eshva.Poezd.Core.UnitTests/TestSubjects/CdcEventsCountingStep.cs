#region Usings

using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Pipeline;
using Microsoft.Extensions.Logging;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public sealed class CdcEventsCountingStep : IStep
  {
    public CdcEventsCountingStep(ILogger<CdcEventsCountingStep> logger)
    {
      _logger = logger;
    }

    public Task Execute(IPocket context)
    {
      _logger.LogInformation(nameof(CdcEventsCountingStep));
      return Task.CompletedTask;
    }

    private readonly ILogger<CdcEventsCountingStep> _logger;
  }
}
