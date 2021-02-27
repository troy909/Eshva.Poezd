#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessageHandling
{
  /// <summary>
  /// Extracts correlation ID, causation ID from broker message headers and sets appropriate metadata in the message handling
  /// context.
  /// </summary>
  public class ExtractRelationMetadataStep : IStep<IPocket>
  {
    public Task Execute(IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      if (!context.TryTake<Dictionary<string, string>>(ContextKeys.Broker.MessageMetadata, out var metadata))
        return Task.CompletedTask;

      if (metadata.TryGetValue(VentureApi.Headers.MessageId, out var messageId))
        context.Put(ContextKeys.Application.MessageId, messageId);

      if (metadata.TryGetValue(VentureApi.Headers.CorrelationId, out var correlationId))
        context.Put(ContextKeys.Application.CorrelationId, correlationId);

      if (metadata.TryGetValue(VentureApi.Headers.CausationId, out var causationId))
        context.Put(ContextKeys.Application.CausationId, causationId);

      return Task.CompletedTask;
    }
  }
}
