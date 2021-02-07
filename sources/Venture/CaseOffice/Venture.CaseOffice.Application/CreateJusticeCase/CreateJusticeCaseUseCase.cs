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


namespace Venture.CaseOffice.Application.CreateJusticeCase
{
  public sealed class CreateJusticeCaseUseCase :
    IHandleMessageOfType<CreateCase>,
    IHavePreconditionsFor<CreateCase>
  {
    public CreateJusticeCaseUseCase(
      IAggregateStorage<JusticeCase> justiceCaseStorage,
      ILogger<CreateJusticeCaseUseCase> logger,
      IIsItJusticeCaseTypeQuery isItJusticeCaseTypeQuery,
      IIsJusticeCaseExistsQuery isJusticeCaseExists)
    {
      _justiceCaseStorage = justiceCaseStorage;
      _logger = logger;
      _isItJusticeCaseTypeQuery = isItJusticeCaseTypeQuery;
      _isJusticeCaseExists = isJusticeCaseExists;
    }

    public async Task Handle(CreateCase message, VentureContext context)
    {
      var justiceCase = new JusticeCase(
        message.CaseId,
        message.SubjectId,
        message.Reason);

      try
      {
        await _justiceCaseStorage.Write(justiceCase);
        context.Commit();
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "fail");
        context.Abort();
      }
    }

    public async Task<bool> ShouldHandle(CreateCase message, VentureContext context) =>
      await _isItJusticeCaseTypeQuery.Execute(message.CaseType) &&
      !await _isJusticeCaseExists.Execute(message.CaseId);

    private readonly IIsItJusticeCaseTypeQuery _isItJusticeCaseTypeQuery;
    private readonly IIsJusticeCaseExistsQuery _isJusticeCaseExists;
    private readonly IAggregateStorage<JusticeCase> _justiceCaseStorage;
    private readonly ILogger<CreateJusticeCaseUseCase> _logger;
  }
}
