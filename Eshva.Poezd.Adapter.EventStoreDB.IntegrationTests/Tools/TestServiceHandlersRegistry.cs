using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  internal class TestServiceHandlersRegistry : IHandlerRegistry
  {
    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
