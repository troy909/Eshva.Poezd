#region Usings

using System;

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
    public Type GetType(string messageTypeName) => GetType();

    /// <inheritdoc />
    public IMessageTypeDescriptor<TMessageType> GetDescriptor<TMessageType>(string messageTypeName)
      where TMessageType : class =>
      new EmptyMessageTypeDescriptor<TMessageType>();

    private class EmptyMessageTypeDescriptor<TMessageType> : IMessageTypeDescriptor<TMessageType> where TMessageType : class
    {
      public TMessageType Parse(Memory<byte> bytes) => Activator.CreateInstance<TMessageType>();

      // ReSharper disable once RedundantAssignment
      public int Serialize(TMessageType message, Span<byte> buffer)
      {
        // ReSharper disable once RedundantAssignment
        buffer = new byte[0];
        return 0;
      }
    }
  }
}
