#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// Base for public API message types registry.
  /// </summary>
  public abstract class MessageTypesRegistry : IMessageTypesRegistry
  {
    /// <inheritdoc />
    public Type GetType(string messageTypeName)
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));

      if (_types.Count == 0)
      {
        throw new PoezdOperationException(
          $"The registry isn't initialized. You have to call {nameof(Initialize)} method before you can use this registry.");
      }

      if (!_types.TryGetValue(messageTypeName, out var type))
        throw new KeyNotFoundException($"Message of type '{messageTypeName}' is unknown.");

      return type;
    }

    /// <inheritdoc />
    public IMessageTypeDescriptor<TMessageType> GetDescriptor<TMessageType>(string messageTypeName) where TMessageType : class
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));

      if (_descriptors.Count == 0)
      {
        throw new PoezdOperationException(
          $"The registry isn't initialized. You have to call {nameof(Initialize)} method before you can use this registry.");
      }

      if (!_descriptors.TryGetValue(messageTypeName, out var descriptor))
        throw new KeyNotFoundException($"Message of type '{messageTypeName}' is unknown.");

      return (IMessageTypeDescriptor<TMessageType>) descriptor;
    }

    /// <summary>
    /// In derived types should be overridden an create message types descriptors and add them using
    /// <see cref="AddDescriptor" />.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Adds message type descriptor into internal descriptors collection.
    /// </summary>
    /// <param name="messageTypeName">
    /// Message type name as taken from a broker message.
    /// </param>
    /// <param name="messageType"></param>
    /// <param name="descriptor">
    /// A message type descriptor to be added.
    /// </param>
    protected void AddDescriptor(
      [NotNull] string messageTypeName,
      [NotNull] Type messageType,
      [NotNull] object descriptor)
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));
      if (messageType == null) throw new ArgumentNullException(nameof(messageType));
      if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));

      _types.Add(messageTypeName, messageType);
      _descriptors.Add(messageTypeName, descriptor);
    }

    private readonly Dictionary<string, object> _descriptors = new Dictionary<string, object>();
    private readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
  }
}
