#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  /// <summary>
  /// A stab message types registry.
  /// </summary>
  /// <remarks>
  /// Used to replace <c>null</c> when message types registry not yet loaded.
  /// </remarks>
  public class EmptyMessageTypesRegistry : IMessageTypesRegistry
  {
    /// <inheritdoc />
    public Type GetMessageTypeByItsMessageTypeName(string messageTypeName) => GetType();

    public string GetMessageTypeNameByItsMessageType<TMessage>() => throw new NotImplementedException();

    /// <inheritdoc />
    public IMessageTypeDescriptor<TMessage> GetDescriptorByMessageTypeName<TMessage>(string messageTypeName)
      where TMessage : class =>
      new EmptyMessageTypeDescriptor<TMessage>();

    public IMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class =>
      new EmptyMessageTypeDescriptor<TMessage>();

    public bool DoesOwn<TMessage>() where TMessage : class => false;

    private class EmptyMessageTypeDescriptor<TMessage> : IMessageTypeDescriptor<TMessage> where TMessage : class
    {
      public IReadOnlyCollection<string> QueueNames { get; } = new string[0];

      public Func<TMessage, object> GetKey { get; } = _ => typeof(TMessage).FullName;

      public TMessage Parse(Memory<byte> bytes) => Activator.CreateInstance<TMessage>();

      // ReSharper disable once RedundantAssignment
      public int Serialize(TMessage message, Span<byte> buffer)
      {
        // ReSharper disable once RedundantAssignment
        buffer = new byte[0];
        return 0;
      }
    }
  }
}
