#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public interface IRedisConnection
  {
    Task<object> SomeQueryById(Guid caseId);
  }
}
