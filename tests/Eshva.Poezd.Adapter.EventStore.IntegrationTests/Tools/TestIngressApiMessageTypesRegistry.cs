#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.EventStore.IntegrationTests.Tools
{
  [UsedImplicitly]
  internal class TestIngressApiMessageTypesRegistry : IngressApiMessageTypesRegistry
  {
    public override void Initialize()
    {
      var messageType = typeof(TestMessage1);
      AddDescriptor(
        messageType.Name,
        messageType,
        new Descriptor());
    }

    private class Descriptor : IIngressMessageTypeDescriptor<TestMessage1>
    {
      public IReadOnlyCollection<string> QueueNames { get; } = new string[0];

      public TestMessage1 Parse(Memory<byte> bytes) => new TestMessage1();
    }
  }
}
