#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter
{
  /// <summary>
  /// Extracts message type from message broker headers and puts message type related items into context.
  /// </summary>
  public class ExtractMessageTypeStep : IStep
  {
    /// <inheritdoc />
    public Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var metadata = context.TakeOrThrow<Dictionary<string, string>>(ContextKeys.Broker.MessageMetadata);

      if (metadata.TryGetValue(VentureApi.Headers.MessageTypeName, out var messageTypeName))
      {
        if (string.IsNullOrWhiteSpace(messageTypeName))
        {
          throw new PoezdOperationException(
            "Message type in its headers is null, an empty or whitespace string. " +
            "By the contract of standard Venture public API it should be specified.");
        }

        context.Put(ContextKeys.Application.MessageTypeName, messageTypeName);
      }
      else
      {
        throw new PoezdOperationException(
          "Can not find the message type in its headers. By the contract of standard Venture public API it should be " +
          $"specified in the {VentureApi.Headers.MessageTypeName} Kafka header.");
      }

      var messageTypesRegistry = context.TakeOrThrow<IPublicApi>(ContextKeys.PublicApi.Itself).MessageTypesRegistry;

      try
      {
        var messageType = messageTypesRegistry.GetMessageTypeByItsMessageTypeName(messageTypeName);
        context.Put(ContextKeys.Application.MessageType, messageType);

        var getDescriptorMethod = typeof(IMessageTypesRegistry).GetMethod(nameof(IMessageTypesRegistry.GetDescriptorByMessageTypeName))!
          .MakeGenericMethod(messageType);

        var descriptor = getDescriptorMethod.Invoke(messageTypesRegistry, new object?[] {messageTypeName});
        context.Put(ContextKeys.Application.MessageTypeDescriptor, descriptor!);
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException("Found an unknown message type. Inspect inner exception to find more information.", exception);
      }

      return Task.CompletedTask;
    }
  }
}
