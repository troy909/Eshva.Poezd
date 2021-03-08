#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessageHandling
{
  /// <summary>
  /// Parses a message object from broker message.
  /// </summary>
  public class ParseBrokerMessageStep : IStep<MessageHandlingContext>
  {
    /// <inheritdoc />
    public Task Execute(MessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));
      if (context.Payload == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.Payload));
      if (context.MessageType == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.MessageType));
      if (context.Descriptor == null) throw context.MakeKeyNotFoundException(nameof(MessageHandlingContext.Descriptor));

      var messagePayload = context.Payload;
      var messageType = context.MessageType;
      var descriptor = context.Descriptor;
      // TODO: Refactor to a private generic method.
      var descriptorType = typeof(IIngressMessageTypeDescriptor<>).MakeGenericType(messageType);
      var parseMethod = descriptorType.GetMethod(nameof(IIngressMessageTypeDescriptor<object>.Parse));
      Memory<byte> buffer = messagePayload;
      context.Message = parseMethod!.Invoke(descriptor, new object?[] {buffer});

      return Task.CompletedTask;
    }
  }
}
