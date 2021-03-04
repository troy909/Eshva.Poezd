using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

namespace Eshva.Poezd.Core.Pipeline
{
  public abstract class EgressMessageTypesRegistry : IEgressMessageTypesRegistry
  {
    /// <inheritdoc />
    public string GetMessageTypeNameByItsMessageType(Type messageType) => _typeToTypeName[messageType];

    /// <inheritdoc />
    public IMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class
    {
      EnsureInitialized();

      var messageType = typeof(TMessage);
      if (!_typeToDescriptor.TryGetValue(messageType, out var descriptor))
        throw new KeyNotFoundException($"Message of type '{messageType.FullName}' is unknown.");

      return (IMessageTypeDescriptor<TMessage>) descriptor;
    }

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

      _typeToTypeName.Add(messageType, messageTypeName);
      _typeToDescriptor.Add(messageType, descriptor);
    }

    private void EnsureInitialized()
    {
      if (_typeToDescriptor.Count == 0)
      {
        throw new PoezdOperationException(
          $"The registry isn't initialized. You have to call {nameof(Initialize)} method before you can use this registry.");
      }
    }

    private readonly Dictionary<Type, object> _typeToDescriptor = new();
    private readonly Dictionary<Type, string> _typeToTypeName = new();
  }
}