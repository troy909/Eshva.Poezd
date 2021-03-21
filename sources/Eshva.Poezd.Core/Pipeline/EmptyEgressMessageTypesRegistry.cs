#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// An empty egress message types registry.
  /// </summary>
  internal sealed class EmptyEgressMessageTypesRegistry : IEgressMessageTypesRegistry
  {
    /// <inheritdoc />
    public string GetMessageTypeNameByItsMessageType(Type messageType) =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");

    /// <inheritdoc />
    public IEgressMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");

    /// <inheritdoc />
    public bool DoesOwn<TMessage>() where TMessage : class =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");
  }
}
