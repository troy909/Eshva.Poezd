#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public interface IRedisConnectionFactory
  {
    Task<IRedisConnection> Connect(string databaseName);
  }
}
