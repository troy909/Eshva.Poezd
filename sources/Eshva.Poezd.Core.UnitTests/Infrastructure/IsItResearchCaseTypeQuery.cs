#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.UnitTests.Application;

#endregion


namespace Eshva.Poezd.Core.UnitTests.Infrastructure
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
