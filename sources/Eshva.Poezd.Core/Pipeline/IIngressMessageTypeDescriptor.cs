using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Eshva.Poezd.Core.Pipeline
{
  public interface IIngressMessageTypeDescriptor<out TMessage> where TMessage : class
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
  }
}