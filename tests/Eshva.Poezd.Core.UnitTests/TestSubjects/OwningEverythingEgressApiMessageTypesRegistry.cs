#region Usings

using System;
using System.Collections.Generic;
using System.Text;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public class OwningEverythingEgressApiMessageTypesRegistry : IEgressApiMessageTypesRegistry
  {
    public string GetMessageTypeNameByItsMessageType(Type messageType) => "fixed-message-type-name";

    public IEgressApiMessageTypeDescriptor<TMessage> GetDescriptorByMessageType<TMessage>() where TMessage : class =>
      new Descriptor<TMessage>("egress-queue-name", message => Encoding.UTF8.GetBytes("fixed-key"));

    public bool DoesOwn<TMessage>() where TMessage : class => true;

    private class Descriptor<TMessage> : IEgressApiMessageTypeDescriptor<TMessage>
      where TMessage : class
    {
      public Descriptor(string queueName, Func<TMessage, object> getKey)
      {
        _queueNames.Add(queueName);
        _getKey = getKey;
      }

      public IReadOnlyCollection<string> QueueNames => _queueNames.AsReadOnly();

      public object GetKey(TMessage message) => _getKey(message);

      public int Serialize(TMessage message, Memory<byte> buffer)
      {
        const string messageContent = "message content";
        Encoding.UTF8.GetBytes(messageContent).AsMemory().CopyTo(buffer);
        return Encoding.UTF8.GetByteCount(messageContent);
      }

      private readonly Func<TMessage, object> _getKey;

      private readonly List<string> _queueNames = new List<string>(capacity: 1);
    }
  }
}
