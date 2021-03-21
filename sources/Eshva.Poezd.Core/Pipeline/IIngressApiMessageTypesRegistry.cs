#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// The contract of an ingress message types registry.
  /// </summary>
  public interface IIngressApiMessageTypesRegistry
  {
    /// <summary>
    /// Gets message CLR-type by message type name.
    /// </summary>
    /// <param name="messageTypeName">
    /// The message type name used to identify a broker message type.
    /// </param>
    /// <returns>
    /// The message CLR-type.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Message type name is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Message type doesn't belongs to this API.
    /// </exception>
    [NotNull]
    Type GetMessageTypeByItsMessageTypeName([NotNull] string messageTypeName);

    /// <summary>
    /// Gets message descriptor by message type name.
    /// </summary>
    /// <param name="messageTypeName">
    /// The message type name used to identify a broker message type.
    /// </param>
    /// <returns>
    /// The found message descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message type name is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Message type doesn't belongs to this API.
    /// </exception>
    [NotNull]
    IIngressMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>([NotNull] string messageTypeName)
      where TMessage : class;
  }
}
