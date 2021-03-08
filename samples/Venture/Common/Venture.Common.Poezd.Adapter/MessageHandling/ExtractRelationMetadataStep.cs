#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessageHandling
{
  /// <summary>
  /// Extracts correlation ID, causation ID from broker message headers and sets appropriate metadata in the message handling
  /// context.
  /// </summary>
  public class ExtractRelationMetadataStep : IStep<MessageHandlingContext>
  {
    public Task Execute(MessageHandlingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var metadata = context.Metadata;
      if (metadata == null) return Task.CompletedTask;

      if (metadata.TryGetValue(VentureApi.Headers.MessageId, out var messageId)) context.MessageId = messageId;
      if (metadata.TryGetValue(VentureApi.Headers.CorrelationId, out var correlationId)) context.CorrelationId = correlationId;
      if (metadata.TryGetValue(VentureApi.Headers.CausationId, out var causationId)) context.CausationId = causationId;

      return Task.CompletedTask;
    }
  }
}
