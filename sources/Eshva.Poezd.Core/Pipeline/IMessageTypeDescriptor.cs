#region Usings

using System;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Contract of a message type descriptor that can serialize and parse messages of <typeparamref name="TMessageType" />.
  /// </summary>
  /// <typeparam name="TMessageType">
  /// The message type this descriptor can serialize and parse.
  /// </typeparam>
  public interface IMessageTypeDescriptor<TMessageType> where TMessageType : class
  {
    /// <summary>
    /// Parses message serialized message from byte array.
    /// </summary>
    /// <param name="bytes">
    /// Serialized message bytes.
    /// </param>
    /// <returns>
    /// Deserialized message object.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Serialized message bytes.
    /// </exception>
    [NotNull]
    TMessageType Parse(Memory<byte> bytes);

    /// <summary>
    /// Serializes a message object into byte array using standard for this public API method.
    /// </summary>
    /// <param name="message">
    /// Message object to be serialized.
    /// </param>
    /// <param name="buffer">
    /// Place where serialized message bytes will be placed.
    /// </param>
    /// <returns>
    /// Number of bytes written.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The <paramref name="message" /> is not specified.
    /// </exception>
    int Serialize([NotNull] TMessageType message, Span<byte> buffer);
  }
}
