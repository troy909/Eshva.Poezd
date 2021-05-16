#region Usings

using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  /// <summary>
  /// Matching nothing queue name matcher.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal sealed class MatchingNothingQueueNameMatcher : IQueueNameMatcher
  {
    /// <inheritdoc />
    public bool DoesMatch(string queueName, string queueNamePattern) => false;
  }
}
