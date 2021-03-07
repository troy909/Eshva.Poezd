#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessageHandling
{
  /// <summary>
  /// Parses a message object from broker message.
  /// </summary>
  public class ParseBrokerMessageStep : IStep<IPocket>
  {
    /// <inheritdoc />
    public Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var messagePayload = context.TakeOrThrow<byte[]>(ContextKeys.Broker.MessagePayload);
      var messageType = context.TakeOrThrow<Type>(ContextKeys.Application.MessageType);
      var descriptor = context.TakeOrThrow<object>(ContextKeys.Application.MessageTypeDescriptor);

      var descriptorType = typeof(IIngressMessageTypeDescriptor<>).MakeGenericType(messageType);
      var parseMethod = descriptorType.GetMethod(nameof(IIngressMessageTypeDescriptor<object>.Parse));
      Memory<byte> buffer = messagePayload;
      var message = parseMethod!.Invoke(descriptor, new object?[] {buffer});
      context.Put(ContextKeys.Application.MessagePayload, message!);

      return Task.CompletedTask;
    }
  }
}
