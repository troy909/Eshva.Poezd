#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// The contract of an egress message type descriptor.
  /// </summary>
  /// <typeparam name="TMessage">
  /// The message type descriptor.
  /// </typeparam>
  public interface IEgressMessageTypeDescriptor<in TMessage> where TMessage : class
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
    /// Gets a functor that returns the key (if required by API or message broker) for the message type.
    /// </summary>
    [NotNull]
    Func<TMessage, byte[]> GetKey { get; }

    /// <summary>
    /// Serializes a message object into byte array using standard for this API method.
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
