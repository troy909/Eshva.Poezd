#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Deserializes message POCO from broker message serialized as FlatBuffers-table using type extracted earlier.
  /// </summary>
  public class DeserializeMessageStep : IStep
  {
    /// <inheritdoc />
    public Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      if (!context.TryTake<byte[]>(ContextKeys.Broker.MessagePayload, out var messagePayload))
        throw new PoezdContextContentException("No broker message payload found in the context.", ContextKeys.Broker.MessagePayload);

      if (!context.TryTake<string>(ContextKeys.Application.MessageTypeName, out var messageTypeName))
        throw new PoezdContextContentException("No application message type name found in the context.", ContextKeys.Application.MessageTypeName);

      var parsed = MessageTypeRegistry.Parse(messageTypeName, messagePayload);
      context.Put(ContextKeys.Application.MessagePayload, parsed);

      return Task.CompletedTask;
    }

    private static readonly MessageTypeRegistry MessageTypeRegistry = new MessageTypeRegistry();
  }
}
