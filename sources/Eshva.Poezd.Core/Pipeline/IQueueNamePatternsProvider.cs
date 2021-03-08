#region Usings

using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Queue name patterns provider.
  /// </summary>
  public interface IQueueNamePatternsProvider
  {
    /// <summary>
    /// Provides queue name patterns that belongs to an API.
    /// </summary>
    /// <returns>
    /// List of queue name patterns.
    /// </returns>
    [NotNull]
    IEnumerable<string> GetQueueNamePatterns();
  }
}
