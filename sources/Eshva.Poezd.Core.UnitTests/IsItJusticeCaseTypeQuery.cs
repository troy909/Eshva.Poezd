#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
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
