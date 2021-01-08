#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Venture.Common.Infrastructure.Redis
{
  public interface IRedisConnection
  {
    Task<object> SomeQueryById(Guid caseId);
  }
}
