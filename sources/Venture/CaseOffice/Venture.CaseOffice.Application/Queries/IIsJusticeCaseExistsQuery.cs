#region Usings

using System;
using System.Threading.Tasks;
using Venture.Common.Application.MessageHandling;

#endregion


namespace Venture.CaseOffice.Application.Queries
{
  public interface IIsJusticeCaseExistsQuery : IQuery
  {
    Task<bool> Execute(Guid caseId);
  }
}
