#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.IntegrationTests.Tools
{
  /// <summary>
  /// An empty egress message types registry.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal sealed class EmptyEgressApiMessageTypesRegistry : IEgressApiMessageTypesRegistry
  {
    /// <inheritdoc />
    public string GetMessageTypeNameByItsMessageType(Type messageType) =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");

    /// <inheritdoc />
    public IEgressApiMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");

    /// <inheritdoc />
    public bool DoesOwn<TMessage>() where TMessage : class =>
      throw new KeyNotFoundException("An empty egress message types registry knows nothing about any message types.");
  }
}
