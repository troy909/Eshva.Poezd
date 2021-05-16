#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// The base of an egress message types registry.
  /// </summary>
  [ExcludeFromCodeCoverage]
  public abstract class EgressApiMessageTypesRegistry : IEgressApiMessageTypesRegistry
  {
    /// <inheritdoc />
    public string GetMessageTypeNameByItsMessageType(Type messageType) => _typeToTypeName[messageType];

    /// <inheritdoc />
    public IEgressApiMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class
    {
      EnsureInitialized();

      var messageType = typeof(TMessage);
      if (!_typeToDescriptor.TryGetValue(messageType, out var descriptor))
        throw new KeyNotFoundException($"Message of type '{messageType.FullName}' is unknown.");

      return (IEgressApiMessageTypeDescriptor<TMessage>) descriptor;
    }

    /// <inheritdoc />
    public bool DoesOwn<TMessage>() where TMessage : class => _typeToDescriptor.ContainsKey(typeof(TMessage));

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
      [JetBrains.Annotations.NotNull] string messageTypeName,
      [JetBrains.Annotations.NotNull] Type messageType,
      [JetBrains.Annotations.NotNull] object descriptor)
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

    private readonly Dictionary<Type, object> _typeToDescriptor = new Dictionary<Type, object>();
    private readonly Dictionary<Type, string> _typeToTypeName = new Dictionary<Type, string>();
  }
}
