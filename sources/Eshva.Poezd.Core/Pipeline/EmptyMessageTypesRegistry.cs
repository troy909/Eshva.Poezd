#region Usings

using System;

#endregion

namespace Eshva.Poezd.Core.Pipeline
{
  public class EmptyMessageTypesRegistry : IMessageTypesRegistry
  {
    public Type GetType(string messageTypeName) => GetType();

    public IMessageTypeDescriptor<TMessageType> GetDescriptor<TMessageType>(string messageTypeName)
      where TMessageType : class =>
      new EmptyMessageTypeDescriptor<TMessageType>();

    private class EmptyMessageTypeDescriptor<TMessageType> : IMessageTypeDescriptor<TMessageType> where TMessageType : class
    {
      public TMessageType Parse(Memory<byte> bytes) => null;

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
