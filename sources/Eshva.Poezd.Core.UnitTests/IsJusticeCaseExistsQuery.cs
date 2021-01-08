#region Usings

using System;
using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public class IsJusticeCaseExistsQuery : IIsJusticeCaseExistsQuery
  {
    public IsJusticeCaseExistsQuery(IRedisConnectionFactory connectionFactory)
    {
      _connectionFactory = connectionFactory;
    }

    public async Task<bool> Execute(Guid caseId)
    {
      var connection = await _connectionFactory.Connect("database name");
      var result = await connection.SomeQueryById(caseId);
      return result != null;
    }

    private readonly IRedisConnectionFactory _connectionFactory;
  }
}
