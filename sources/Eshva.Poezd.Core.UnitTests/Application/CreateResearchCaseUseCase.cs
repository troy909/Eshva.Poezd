#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.UnitTests.Domain;
using Eshva.Poezd.Core.UnitTests.Infrastructure;
using Eshva.Poezd.Core.UnitTests.Messages;

#endregion


namespace Eshva.Poezd.Core.UnitTests.Application
{
  public sealed class CreateResearchCaseUseCase :
    IHandleMessageOfType<CreateCase>,
    IHavePreconditionsFor<CreateCase>
  {
    public CreateResearchCaseUseCase(
      IAggregateStorage<ResearchCase> researchCaseStorage,
      ILogger<CreateResearchCaseUseCase> logger,
      IsItResearchCaseTypeQuery isItResearchCaseTypeQuery)
    {
      _researchCaseStorage = researchCaseStorage;
      _logger = logger;
      _isItResearchCaseTypeQuery = isItResearchCaseTypeQuery;
    }

    public async Task Handle(CreateCase message, SampleMessageHandlingContext context)
    {
      var researchCase = new ResearchCase(message.CaseId, message.SubjectId, message.Reason);

      try
      {
        await _researchCaseStorage.Write(researchCase);
        context.Commit();
      }
      catch (Exception exception)
      {
        _logger.LogException(exception);
        context.Abort();
      }
    }

    public Task<bool> ShouldHandle(CreateCase message, SampleMessageHandlingContext context) =>
      _isItResearchCaseTypeQuery.Execute(message.CaseType);

    private readonly ILogger<CreateResearchCaseUseCase> _logger;
    private readonly IsItResearchCaseTypeQuery _isItResearchCaseTypeQuery;
    private readonly IAggregateStorage<ResearchCase> _researchCaseStorage;
  }
}
