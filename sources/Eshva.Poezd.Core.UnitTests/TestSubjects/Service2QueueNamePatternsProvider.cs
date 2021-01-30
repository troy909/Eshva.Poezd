#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class Service2QueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    public IEnumerable<string> GetQueueNamePatterns()
    {
      yield return "sample.facts.service-2.v1";
    }
  }
}
