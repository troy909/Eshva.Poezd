#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  [UsedImplicitly]
  internal class TestServiceHandlersRegistry : IHandlerRegistry
  {
    public TestServiceHandlersRegistry()
    {
      HandlersGroupedByMessageType = new[] {Assembly.GetExecutingAssembly()}
        .GetHandlersGroupedByMessageType(typeof(IMessageHandler<>))
        .ToDictionary(pair => pair.Key, pair => pair.Value);
    }

    public IReadOnlyDictionary<Type, Type[]> HandlersGroupedByMessageType { get; }
  }
}
