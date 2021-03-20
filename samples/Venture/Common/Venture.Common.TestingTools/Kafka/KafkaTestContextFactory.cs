#region Usings

using System;
using System.Threading;

#endregion

namespace Venture.Common.TestingTools.Kafka
{
  public class KafkaTestContextFactory
  {
    public KafkaTestContextFactory(string bootstrapServers)
    {
      if (string.IsNullOrWhiteSpace(bootstrapServers))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(bootstrapServers));

      _bootstrapServers = bootstrapServers;
    }

    public KafkaTestContext<TKey, TValue> Create<TKey, TValue>(CancellationToken cancellationToken = default) =>
      new KafkaTestContext<TKey, TValue>(_bootstrapServers, cancellationToken);

    private readonly string _bootstrapServers;
  }
}
