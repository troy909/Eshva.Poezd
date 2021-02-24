#region Usings

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Venture.CaseOffice.Domain;
using Venture.CaseOffice.Messages.V1.Commands;
using Venture.Common.Application.MessageHandling;
using Venture.Common.Application.Storage;

#endregion

namespace Venture.CaseOffice.Application.CreateJusticeCaseProcess
{
  public sealed class CreateJusticeCaseUseCase : IHandleMessageOfType<CreateJusticeCase>
  {
    public CreateJusticeCaseUseCase(
      IAggregateStorage<JusticeCase> justiceCaseStorage,
      ILogger<CreateJusticeCaseUseCase> logger)
    {
      _justiceCaseStorage = justiceCaseStorage;
      _logger = logger;
    }

    public async Task Handle(CreateJusticeCase message, VentureContext context)
    {
      var justiceCase = new JusticeCase(
        Guid.NewGuid(),
        message.SubjectId,
        message.Reason,
        message.ResposibleId);

      try
      {
        await _justiceCaseStorage.Write(justiceCase.CaseId, justiceCase);
        context.Commit();
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "fail");
        context.Abort();
      }
    }

    private readonly IAggregateStorage<JusticeCase> _justiceCaseStorage;
    private readonly ILogger<CreateJusticeCaseUseCase> _logger;
  }
}
