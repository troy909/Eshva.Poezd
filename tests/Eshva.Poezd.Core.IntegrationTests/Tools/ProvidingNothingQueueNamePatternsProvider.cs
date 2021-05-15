#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.IntegrationTests.Tools
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
