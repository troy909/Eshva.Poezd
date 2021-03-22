namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Contract of queue name patterns matcher of some kind. Implementations should define pattern matching algorithm.
  /// </summary>
  public interface IQueueNameMatcher
  {
    /// <summary>
    /// Checks is queue name matches the pattern.
    /// </summary>
    /// <param name="queueName">
    /// Queue name to check.
    /// </param>
    /// <param name="queueNamePattern">
    /// The pattern to which queue name is matched.
    /// </param>
    /// <returns>
    /// <c>true</c> - queue name matches the pattern, <c>false</c> - otherwise.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// One of arguments is null, empty or whitespace only string.
    /// </exception>
    /// <exception cref="System.ArgumentNullException">
    /// A regular expression parsing error occurred.
    /// </exception>
    /// <exception cref="System.OverflowException">
    /// Too many patterns used.
    /// </exception>
    bool DoesMatch(string queueName, string queueNamePattern);
  }
}
