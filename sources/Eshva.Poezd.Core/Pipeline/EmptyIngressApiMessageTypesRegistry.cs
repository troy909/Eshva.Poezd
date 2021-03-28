#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// An empty ingress message types registry.
  /// </summary>
  [ExcludeFromCodeCoverage]
  internal sealed class EmptyIngressApiMessageTypesRegistry : IIngressApiMessageTypesRegistry
  {
    /// <inheritdoc />
    public Type GetMessageTypeByItsMessageTypeName(string messageTypeName) =>
      throw new KeyNotFoundException("An empty ingress message types registry knows nothing about any message types.");

    /// <inheritdoc />
    public IIngressMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(string messageTypeName)
      where TMessage : class =>
      throw new KeyNotFoundException("An empty ingress message types registry knows nothing about any message types.");
  }
}
