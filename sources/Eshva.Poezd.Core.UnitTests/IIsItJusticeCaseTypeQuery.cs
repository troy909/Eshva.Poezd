#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public interface IIsItJusticeCaseTypeQuery : IQuery
  {
    Task<bool> Execute(string caseType);
  }
}
