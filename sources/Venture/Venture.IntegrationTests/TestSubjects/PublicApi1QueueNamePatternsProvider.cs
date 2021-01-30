#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class PublicApi1QueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    public IEnumerable<string> GetQueueNamePatterns()
    {
      yield return "^some-";
    }
  }
}
