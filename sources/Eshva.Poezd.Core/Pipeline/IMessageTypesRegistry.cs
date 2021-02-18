#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Contract of a message types registry of an asynchronous public API.
  /// </summary>
  public interface IMessageTypesRegistry
  {
    /// <summary>
    /// Gets CLR message type by broker message type.
    /// </summary>
    /// <param name="messageTypeName"></param>
    /// <returns></returns>
    [NotNull]
    Type GetType([NotNull] string messageTypeName);

    /// <summary>
    /// Matches message type taken from broker message headers to the message type object.
    /// </summary>
    /// <param name="messageTypeName">
    /// Message type name as taken from a broker message.
    /// </param>
    /// <returns>
    /// The found message type object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message type is null, an empty or whitespace string.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Message type isn't belongs to this public API.
    /// </exception>
    [NotNull]
    IMessageTypeDescriptor<TMessageType> GetDescriptor<TMessageType>([NotNull] string messageTypeName) where TMessageType : class;
  }
}
