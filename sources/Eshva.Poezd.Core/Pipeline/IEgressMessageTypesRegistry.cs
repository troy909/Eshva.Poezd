#region Usings

using System;
using System.Collections.Generic;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  public interface IEgressMessageTypesRegistry
  {
    /// <summary>
    /// Gets message type name by CLR-message type.
    /// </summary>
    /// <param name="messageType">
    /// THe message type.
    /// </param>
    /// <returns>
    /// The message type name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Message type is not specified.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Message type doesn't belongs to this API.
    /// </exception>
    [NotNull]
    string GetMessageTypeNameByItsMessageType(Type messageType);

    /// <summary>
    /// Gets message descriptor be the message CLR-Type.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The message CLR-type.
    /// </typeparam>
    /// <returns>
    /// The found message descriptor.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// The message type is not specified.
    /// </exception>
    /// <exception cref="KeyNotFoundException">
    /// Message type doesn't belongs to this API.
    /// </exception>
    [NotNull]
    IEgressMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class;

    /// <summary>
    /// Checks if message of <typeparamref name="TMessage" /> belongs to this API.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The message CLR-type.
    /// </typeparam>
    /// <returns>
    /// Returns <c>true</c> if message belongs to this API, <c>false</c> otherwise.
    /// </returns>
    bool DoesOwn<TMessage>() where TMessage : class;
  }
}
