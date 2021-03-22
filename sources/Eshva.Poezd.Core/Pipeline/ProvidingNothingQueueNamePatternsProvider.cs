#region Usings

using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// An empty collection of message queue names.
  /// </summary>
  internal sealed class ProvidingNothingQueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    /// <inheritdoc />
    public IEnumerable<string> GetQueueNamePatterns()
    {
      yield break;
    }
  }
}
