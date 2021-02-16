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
      // IMPORTANT: All topics should exist to subscribe to any of them. Otherwise no messages received if you subscribe to a few.
      return new[] { @"^venture\.(commands|facts)\..*" };
    }
  }
}
