#region Usings

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Venture.CaseOffice.Domain;
using Venture.CaseOffice.Messages.V1.Commands;
using Venture.Common.Application.Ingress;
using Venture.Common.Application.Storage;

#endregion

namespace Venture.CaseOffice.Application.CreateResearchCaseProcess
{
  public sealed class CreateResearchCaseUseCase : IMessageHandler<CreateResearchCase>
  {
    public CreateResearchCaseUseCase(
      IAggregateStorage<ResearchCase> researchCaseStorage,
      ILogger<CreateResearchCaseUseCase> logger)
    {
      _researchCaseStorage = researchCaseStorage;
      _logger = logger;
    }

    public async Task Handle(CreateResearchCase message, VentureIncomingMessageHandlingContext context)
    {
      var researchCase = new ResearchCase(
        Guid.NewGuid(),
        message.Reason,
        message.KnowledgeArea);

      try
      {
        await _researchCaseStorage.Write(researchCase.Id, researchCase);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "fail");
      }
    }

    private readonly ILogger<CreateResearchCaseUseCase> _logger;
    private readonly IAggregateStorage<ResearchCase> _researchCaseStorage;
  }
}
