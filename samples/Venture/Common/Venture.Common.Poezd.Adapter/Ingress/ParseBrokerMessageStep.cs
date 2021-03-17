#region Usings

using System;
using System.Reflection;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.Ingress
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

      var concreteMethod = ParseMessageMethod.MakeGenericMethod(context.MessageType);
      context.Message = concreteMethod.Invoke(this, new object?[] {context});
      return Task.CompletedTask;
    }

    private TMessage ParseMessage<TMessage>(MessageHandlingContext context) where TMessage : class
    {
      var descriptor = (IIngressMessageTypeDescriptor<TMessage>) context.Descriptor;
      Memory<byte> buffer = (byte[]) context.Payload;
      return descriptor.Parse(buffer);
    }

    private static readonly MethodInfo ParseMessageMethod =
      typeof(ParseBrokerMessageStep).GetMethod(nameof(ParseMessage), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
