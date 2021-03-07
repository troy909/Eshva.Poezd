#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  public abstract class IngressMessageTypesRegistry : IIngressMessageTypesRegistry
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

    /// <summary>
    /// In derived types should be overridden an create message types descriptors and add them using
    /// <see cref="AddDescriptor" />.
    /// </summary>
    public abstract void Initialize();

    public IIngressMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(string messageTypeName) where TMessage : class
    {
      if (string.IsNullOrWhiteSpace(messageTypeName)) throw new ArgumentNullException(nameof(messageTypeName));

      EnsureInitialized();

      if (!_typeNameToDescriptor.TryGetValue(messageTypeName, out var descriptor))
        throw new KeyNotFoundException($"Message of type '{messageTypeName}' is unknown.");

      return (IIngressMessageTypeDescriptor<TMessage>) descriptor;
    }

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
