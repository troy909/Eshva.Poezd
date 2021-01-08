#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests.Application
{
  public interface IIsItResearchCaseTypeQuery : IQuery
  {
    Task<bool> Execute(string caseType);
  }
}
