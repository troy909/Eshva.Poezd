#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  internal sealed class EmptyIngressMessageTypesRegistry : IIngressMessageTypesRegistry
  {
    public Type GetMessageTypeByItsMessageTypeName(string messageTypeName) =>
      throw new KeyNotFoundException("An empty ingress message types registry knows nothing about any message types.");

    public IIngressMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(string messageTypeName)
      where TMessage : class =>
      throw new KeyNotFoundException("An empty ingress message types registry knows nothing about any message types.");
  }
}
