using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Eshva.Poezd.Core.Pipeline
{
  public interface IIngressMessageTypesRegistry
  {
    /// <summary>
    /// Gets message CLR-type by message type name.
    /// </summary>
    /// <param name="messageTypeName"></param>
    /// <returns>
    /// The message CLR-type.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Message type name is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Message type doesn't belongs to this public API.
    /// </exception>
    [NotNull]
    Type GetMessageTypeByItsMessageTypeName([NotNull] string messageTypeName);

    /// <summary>
    /// Gets message descriptor by message type name.
    /// </summary>
    /// <param name="messageTypeName">
    /// Message type name from a broker message metadata.
    /// </param>
    /// <returns>
    /// The found message descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message type name is <c>null</c>, an empty or a whitespace string.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Message type doesn't belongs to this public API.
    /// </exception>
    [NotNull]
    IMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>([NotNull] string messageTypeName) where TMessage : class;
  }
}