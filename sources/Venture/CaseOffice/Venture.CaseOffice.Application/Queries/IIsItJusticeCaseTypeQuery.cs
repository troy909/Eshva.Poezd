#region Usings

using System.Threading.Tasks;
using Venture.Common.Application.MessageHandling;

#endregion

namespace Venture.CaseOffice.Application.Queries
{
  public interface IIsItJusticeCaseTypeQuery : IQuery
  {
    Task<bool> Execute(string caseType);
  }
}
