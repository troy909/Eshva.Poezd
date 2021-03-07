namespace Eshva.Poezd.Core.Routing
{
  internal sealed class MatchingNothingQueueNameMatcher : IQueueNameMatcher
  {
    public bool DoesMatch(string queueName, string queueNamePattern) => false;
  }
}
