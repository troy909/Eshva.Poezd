#region Usings

using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  // ReSharper disable once ClassNeverInstantiated.Global
  public class MatchingEverythingQueueNameMatcher : IQueueNameMatcher
  {
    public bool DoesMatch(string queueName, string queueNamePattern) => true;
  }
}
