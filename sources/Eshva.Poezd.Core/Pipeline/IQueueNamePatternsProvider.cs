#region Usings

using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  public interface IQueueNamePatternsProvider
  {
    IEnumerable<string> GetQueueNamePatterns();
  }
}
