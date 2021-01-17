namespace Eshva.Poezd.Core.Routing
{
  public interface IQueueNameMatcher
  {
    bool IsMatch(string queueName, string queueNamePattern);
  }
}