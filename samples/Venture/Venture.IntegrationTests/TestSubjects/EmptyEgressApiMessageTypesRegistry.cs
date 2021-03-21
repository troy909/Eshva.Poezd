#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Venture.IntegrationTests.TestSubjects
{
  internal class EmptyEgressApiMessageTypesRegistry : IEgressApiMessageTypesRegistry
  {
    public string GetMessageTypeNameByItsMessageType(Type messageType) =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");

    public IEgressApiMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");

    public bool DoesOwn<TMessage>() where TMessage : class =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");
  }
}
