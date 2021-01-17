#region Usings

using System;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Queue name that uses regex pattern matching.
  /// </summary>
  [UsedImplicitly]
  public sealed class RegexQueueNameMatcher : IQueueNameMatcher
  {
    /// <inheritdoc />
    public bool DoesMatch([NotNull] string queueName, [NotNull] string queueNamePattern)
    {
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(NotWhitespace, nameof(queueName));
      if (string.IsNullOrWhiteSpace(queueNamePattern)) throw new ArgumentNullException(NotWhitespace, nameof(queueNamePattern));

      if (!queueNamePattern.StartsWith('^'))
      {
        return queueName.Equals(queueNamePattern, StringComparison.InvariantCulture);
      }

      var regex = KnownRegex.GetOrAdd(
        queueNamePattern,
        pattern => new Regex(queueNamePattern, RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant));
      return regex.IsMatch(queueName);
    }

    private readonly ConcurrentDictionary<string, Regex> KnownRegex = new ConcurrentDictionary<string, Regex>();
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
