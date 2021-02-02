#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Extracts correlation ID, causation ID from broker message headers and sets appropriate metadata in the message handling
  /// context.
  /// </summary>
  public class ExtractRelationMetadataStep : IStep
  {
    public Task Execute([NotNull] IPocket context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      if (!context.TryTake<Dictionary<string, string>>(ContextKeys.Broker.MessageMetadata, out var metadata))
        return Task.CompletedTask;

      if (metadata.TryGetValue(WorkPlannerApi.Headers.MessageId, out var messageId))
        context.Put(ContextKeys.Application.MessageId, messageId);

      if (metadata.TryGetValue(WorkPlannerApi.Headers.CorrelationId, out var correlationId))
        context.Put(ContextKeys.Application.CorrelationId, correlationId);

      if (metadata.TryGetValue(WorkPlannerApi.Headers.CausationId, out var causationId))
        context.Put(ContextKeys.Application.CorrelationId, causationId);

      return Task.CompletedTask;
    }
  }
}
