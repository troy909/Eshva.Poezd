#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// The base of an ingress message types registry.
  /// </summary>
  public abstract class IngressApiMessageTypesRegistry : IIngressApiMessageTypesRegistry
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
    public IIngressMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(string messageTypeName)
      where TMessage : class
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));

      EnsureInitialized();

      if (!_typeNameToDescriptor.TryGetValue(messageTypeName, out var descriptor))
        throw new KeyNotFoundException($"Message of type '{messageTypeName}' is unknown.");

      return (IIngressMessageTypeDescriptor<TMessage>) descriptor;
    }

    /// <summary>
    /// In derived types should be overridden an create message types descriptors and add them using
    /// <see cref="AddDescriptor" />.
    /// </summary>
    public abstract void Initialize();

    /// <summary>
    /// Adds a message type descriptor into internal descriptors collection.
    /// </summary>
    /// <param name="messageTypeName">
    /// The message type name used to identify a broker message type.
    /// </param>
    /// <param name="messageType">
    /// The message CLR-type.
    /// </param>
    /// <param name="descriptor">
    /// The message type descriptor to be added.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    protected void AddDescriptor(
      [NotNull] string messageTypeName,
      [NotNull] Type messageType,
      [NotNull] object descriptor)
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));
      if (messageType == null) throw new ArgumentNullException(nameof(messageType));
      if (descriptor == null) throw new ArgumentNullException(nameof(descriptor));

      _typeNameToType.Add(messageTypeName, messageType);
      _typeNameToDescriptor.Add(messageTypeName, descriptor);
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
  }
}
