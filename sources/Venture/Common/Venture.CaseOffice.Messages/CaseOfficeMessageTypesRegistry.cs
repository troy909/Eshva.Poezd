#region Usings

using System;
using System.Linq;
using Eshva.Poezd.Core.Pipeline;
using FlatSharp;

#endregion

namespace Venture.CaseOffice.Messages
{
  public sealed class CaseOfficeMessageTypesRegistry : MessageTypesRegistry
  {
    public override void Initialize()
    {
      var messageTypes = GetType().Assembly.ExportedTypes
        .Where(type => type.FullName!.StartsWith("Venture.CaseOffice.Messages.V1.", StringComparison.InvariantCulture));

      foreach (var messageType in messageTypes)
      {
        var descriptorType = typeof(MessageTypeDescriptor<>).MakeGenericType(messageType);
        // IMPORTANT: It's the fastest method to create an instance of a type with no parameters.
        // https://github.com/agileobjects/eg-create-instance-from-type
        var descriptor = Activator.CreateInstance(descriptorType);
        AddDescriptor(
          messageType.FullName!,
          messageType,
          descriptor ?? throw new InvalidOperationException($"Can not create a descriptor for message type {messageType.FullName}."));
      }
    }

    private class MessageTypeDescriptor<TMessageType> : IMessageTypeDescriptor<TMessageType>
      where TMessageType : class
    {
      public TMessageType Parse(Memory<byte> bytes) => FlatBufferSerializer.Default.Parse<TMessageType>(bytes);

      public int Serialize(TMessageType message, Span<byte> buffer) => FlatBufferSerializer.Default.Serialize(message, buffer);
    }
  }
}
