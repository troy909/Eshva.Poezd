#region Usings

using System;
using System.Threading.Tasks;
using Venture.CaseOffice.Application.Queries;
using Venture.Common.Infrastructure.Redis;

#endregion


namespace Venture.CaseOffice.ReadModelStorage.Redis.JusticeCase
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
