#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public sealed class CreateJusticeCaseUseCase :
    IHandleMessageOfType<CreateCase>,
    IHavePreconditionsFor<CreateCase>
  {
    public CreateJusticeCaseUseCase(
      IAggregateStorage<JusticeCase> justiceCaseStorage,
      ILogger<CreateJusticeCaseUseCase> logger,
      IsItJusticeCaseTypeQuery isItJusticeCaseTypeQuery,
      IsJusticeCaseExistsQuery isJusticeCaseExists)
    {
      _justiceCaseStorage = justiceCaseStorage;
      _logger = logger;
      _isItJusticeCaseTypeQuery = isItJusticeCaseTypeQuery;
      _isJusticeCaseExists = isJusticeCaseExists;
    }

    public async Task Handle(CreateCase message, IMessageHandlingContext context)
    {
      var justiceCase = new JusticeCase(message.CaseId, message.SubjectId, message.Reason);

      try
      {
        await _justiceCaseStorage.Write(justiceCase);
        context.Commit();
      }
      catch (Exception exception)
      {
        _logger.LogException(exception);
        context.Uncommit();
      }
    }

    public async Task<bool> ShouldHandle(CreateCase message, IMessageHandlingContext context) =>
      await _isItJusticeCaseTypeQuery.Execute(message.CaseType) &&
      !await _isJusticeCaseExists.Execute(message.CaseId);

    private readonly IsItJusticeCaseTypeQuery _isItJusticeCaseTypeQuery;
    private readonly IsJusticeCaseExistsQuery _isJusticeCaseExists;
    private readonly IAggregateStorage<JusticeCase> _justiceCaseStorage;
    private readonly ILogger<CreateJusticeCaseUseCase> _logger;
  }
}
