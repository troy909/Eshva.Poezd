#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
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
