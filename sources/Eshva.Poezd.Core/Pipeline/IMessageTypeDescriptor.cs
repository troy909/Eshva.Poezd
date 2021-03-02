#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Contract of a message type descriptor that can serialize and parse messages of <typeparamref name="TMessage" />.
  /// </summary>
  /// <typeparam name="TMessage">
  /// The message type this descriptor can serialize and parse.
  /// </typeparam>
  public interface IMessageTypeDescriptor<TMessage> where TMessage : class
  {
    /// <summary>
    /// Gets queue names to which this message type belongs and should be published to.
    /// </summary>
    /// <remarks>
    /// For a good designed API it should contain exactly one queue name.
    /// </remarks>
    [NotNull]
    IReadOnlyCollection<string> QueueNames { get; }

    /// <summary>
    /// Gets a functor that returns the partition key (if required by public API or message broker) for the message type.
    /// </summary>
    [NotNull]
    Func<TMessage, object> GetKey { get; }

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
    TMessage Parse(Memory<byte> bytes);

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
    int Serialize([NotNull] TMessage message, Memory<byte> buffer);
  }
}
