#region Usings

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Venture.CaseOffice.Domain;
using Venture.CaseOffice.Messages.V1.Commands;
using Venture.Common.Application.Ingress;
using Venture.Common.Application.Storage;

#endregion

namespace Venture.CaseOffice.Application.CreateJusticeCaseProcess
{
  public sealed class CreateJusticeCaseUseCase : IMessageHandler<CreateJusticeCase>
  {
    public CreateJusticeCaseUseCase(
      IAggregateStorage<JusticeCase> justiceCaseStorage,
      ILogger<CreateJusticeCaseUseCase> logger)
    {
      _justiceCaseStorage = justiceCaseStorage;
      _logger = logger;
    }

    public async Task Handle(CreateJusticeCase message, VentureIncomingMessageHandlingContext context)
    {
      var justiceCase = new JusticeCase(
        Guid.NewGuid(),
        message.SubjectId,
        message.Reason,
        message.ResponsibleId);

      try
      {
        await _justiceCaseStorage.Write(justiceCase.CaseId, justiceCase);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "fail");
      }
    }

    private readonly IAggregateStorage<JusticeCase> _justiceCaseStorage;
    private readonly ILogger<CreateJusticeCaseUseCase> _logger;
  }
}
