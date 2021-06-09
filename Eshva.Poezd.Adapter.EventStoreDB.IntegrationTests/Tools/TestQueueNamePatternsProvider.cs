#region Usings

using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  [UsedImplicitly]
  internal class TestQueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    public IEnumerable<string> GetQueueNamePatterns() => new[] {TestAggregateCategoryStreamName};

    public const string TestAggregateCategoryStreamName = "$ce-TestAggregate";
  }
}
