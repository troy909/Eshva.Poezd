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
    public Type GetMessageTypeByItsMessageTypeName(string messageTypeName)
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));

      EnsureInitialized();

      if (!_typeNameToType.TryGetValue(messageTypeName, out var type))
        throw new KeyNotFoundException($"Message of type '{messageTypeName}' is unknown.");

      return type;
    }

    /// <inheritdoc />
    public string GetMessageTypeNameByItsMessageType<TMessage>() => _typeToTypeName[typeof(TMessage)];

    /// <inheritdoc />
    public IMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(string messageTypeName) where TMessage : class
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));

      EnsureInitialized();

      if (!_typeNameToDescriptor.TryGetValue(messageTypeName, out var descriptor))
        throw new KeyNotFoundException($"Message of type '{messageTypeName}' is unknown.");

      return (IMessageTypeDescriptor<TMessage>) descriptor;
    }

    /// <inheritdoc />
    public IMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class
    {
      EnsureInitialized();

      var messageType = typeof(TMessage);
      if (!_typeToDescriptor.TryGetValue(messageType, out var descriptor))
        throw new KeyNotFoundException($"Message of type '{messageType.FullName}' is unknown.");

      return (IMessageTypeDescriptor<TMessage>) descriptor;
    }

    /// <inheritdoc />
    public bool DoesOwn<TMessage>() where TMessage : class => _typeToDescriptor.ContainsKey(typeof(TMessage));

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

      _typeNameToType.Add(messageTypeName, messageType);
      _typeToTypeName.Add(messageType, messageTypeName);
      _typeNameToDescriptor.Add(messageTypeName, descriptor);
      _typeToDescriptor.Add(messageType, descriptor);
    }

    private void EnsureInitialized()
    {
      if (_typeNameToDescriptor.Count == 0)
      {
        throw new PoezdOperationException(
          $"The registry isn't initialized. You have to call {nameof(Initialize)} method before you can use this registry.");
      }
    }

    private readonly Dictionary<string, object> _typeNameToDescriptor = new();
    private readonly Dictionary<string, Type> _typeNameToType = new();
    private readonly Dictionary<Type, object> _typeToDescriptor = new();
    private readonly Dictionary<Type, string> _typeToTypeName = new();
  }
}
