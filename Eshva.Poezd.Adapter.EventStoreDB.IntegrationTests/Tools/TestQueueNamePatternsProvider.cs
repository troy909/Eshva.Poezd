using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  internal class TestQueueNamePatternsProvider : IQueueNamePatternsProvider
  {
    public IEnumerable<string> GetQueueNamePatterns() => throw new NotImplementedException();
  }
}
