#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public interface IAggregateStorage<TAggregate> where TAggregate : class
  {
    Task<TAggregate> Read(Guid caseId);

    Task Write(TAggregate aggregate);
  }
}
