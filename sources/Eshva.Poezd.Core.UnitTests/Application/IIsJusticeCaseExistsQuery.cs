#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests.Application
{
  public interface IIsJusticeCaseExistsQuery : IQuery
  {
    Task<bool> Execute(Guid caseId);
  }
}
