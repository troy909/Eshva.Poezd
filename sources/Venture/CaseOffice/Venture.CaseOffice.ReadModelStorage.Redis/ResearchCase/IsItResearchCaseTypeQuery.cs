#region Usings

using System;
using System.Threading.Tasks;
using Venture.CaseOffice.Application.Queries;

#endregion

namespace Venture.CaseOffice.ReadModelStorage.Redis.ResearchCase
{
  public class IsItResearchCaseTypeQuery : IIsItResearchCaseTypeQuery
  {
    public Task<bool> Execute(string caseType)
    {
      const string ResearchCaseType = "Research15";
      return Task.FromResult(caseType.Equals(ResearchCaseType, StringComparison.InvariantCultureIgnoreCase));
    }
  }
}
