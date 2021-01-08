#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests.Infrastructure
{
  public interface IRedisConnectionFactory
  {
    Task<IRedisConnection> Connect(string databaseName);
  }
}
