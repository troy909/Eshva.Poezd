#region Usings

using System;
using System.Threading.Tasks;
using Venture.CaseOffice.Application.Queries;

#endregion

namespace Venture.CaseOffice.ReadModelStorage.Redis.JusticeCase
{
  public class IsItJusticeCaseTypeQuery : IIsItJusticeCaseTypeQuery
  {
    public Task<bool> Execute(string caseType)
    {
      const string JusticeCaseType = "JUST";
      return Task.FromResult(caseType.Equals(JusticeCaseType, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
