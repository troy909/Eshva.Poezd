#region Usings

using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  internal sealed class ProvidingNothingQueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    public IEnumerable<string> GetQueueNamePatterns()
    {
      yield break;
    }
  }
}
