#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Venture.Common.Application.Storage
{
  public interface IAggregateStorage<TAggregate> where TAggregate : class
  {
    Task<TAggregate> Read(Guid caseId);

    Task Write(TAggregate aggregate);
  }
}
