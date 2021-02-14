#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  public class VentureQueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    public IEnumerable<string> GetQueueNamePatterns()
    {
      yield return "^venture.commands..*";
      yield return "^venture.facts..*";
    }
  }
}
