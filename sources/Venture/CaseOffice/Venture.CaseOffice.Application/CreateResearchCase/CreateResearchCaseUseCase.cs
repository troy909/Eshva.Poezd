#region Usings

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Venture.CaseOffice.Application.Queries;
using Venture.CaseOffice.Domain;
using Venture.CaseOffice.Public;
using Venture.Common.Application.MessageHandling;
using Venture.Common.Application.Storage;

#endregion


namespace Venture.CaseOffice.Application.CreateResearchCase
{
  public sealed class CreateResearchCaseUseCase :
    IHandleMessageOfType<CreateCase>,
    IHavePreconditionsFor<CreateCase>
  {
    public CreateResearchCaseUseCase(
      IAggregateStorage<ResearchCase> researchCaseStorage,
      ILogger<CreateResearchCaseUseCase> logger,
      IIsItResearchCaseTypeQuery isItResearchCaseTypeQuery)
    {
      _researchCaseStorage = researchCaseStorage;
      _logger = logger;
      _isItResearchCaseTypeQuery = isItResearchCaseTypeQuery;
    }

    public async Task Handle(CreateCase message, VentureContext context)
    {
      var researchCase = new ResearchCase(
        message.CaseId,
        message.SubjectId,
        message.Reason);

      try
      {
        await _researchCaseStorage.Write(researchCase);
        context.Commit();
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "fail");
        context.Abort();
      }
    }

    public Task<bool> ShouldHandle(CreateCase message, VentureContext context) =>
      _isItResearchCaseTypeQuery.Execute(message.CaseType);

    private readonly IIsItResearchCaseTypeQuery _isItResearchCaseTypeQuery;

    private readonly ILogger<CreateResearchCaseUseCase> _logger;
    private readonly IAggregateStorage<ResearchCase> _researchCaseStorage;
  }
}
