using System;
using Eshva.Poezd.Core.Pipeline;

namespace Eshva.Poezd.Adapter.EventStoreDB.IntegrationTests.Tools
{
  internal class TestIngressApiMessageTypesRegistry : IIngressApiMessageTypesRegistry
  {
    public Type GetMessageTypeByItsMessageTypeName(string messageTypeName) => throw new NotImplementedException();

    public IIngressMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(string messageTypeName)
      where TMessage : class => throw new NotImplementedException();
  }
}
