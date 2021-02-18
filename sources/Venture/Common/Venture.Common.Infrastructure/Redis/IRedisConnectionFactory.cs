#region Usings

using System.Threading.Tasks;

#endregion

namespace Venture.Common.Infrastructure.Redis
{
  public interface IRedisConnectionFactory
  {
    Task<IRedisConnection> Connect(string databaseName);
  }
}
