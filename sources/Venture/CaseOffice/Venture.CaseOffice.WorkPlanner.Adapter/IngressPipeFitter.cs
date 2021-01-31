#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Venture.CaseOffice.WorkPlanner.Adapter
{
  /// <summary>
  /// Work Planner service API ingress pipe fitter.
  /// </summary>
  /// <remarks>
  /// Work Planner service API contract:
  /// <list type="bullet">
  /// <item>
  /// <descriptin>uses Kafka to send/receive messages;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>publishes events of its aggregates to topics named <c>case.facts.[aggregate-name].v1</c>;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>sends message type in a broker header named <c>Type</c>;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>serializes its messages using FlatBuffers;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>sends correlation ID in a broker header named <c>CorrelationId</c>;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>sends causation ID in a broker header named <c>CausationId</c>;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>all its events contains <c>Metadata</c> and <c>Payload</c> fields;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>
  /// its message <c>Metadata</c> field contains <c>CorrelationId</c>, <c>CausationId</c>, <c>OriginatorToken</c> if known;
  /// </descriptin>
  /// </item>
  /// <item>
  /// <descriptin>
  /// its <c>OriginatorToken</c> metadata field contains JWT token which type is <see cref="OriginatorToken" />;
  /// </descriptin>
  /// </item>
  /// <item>
  /// <descriptin>TODO: Specify other contract details if needed.</descriptin>
  /// </item>
  /// </list>
  /// </remarks>
  public sealed class IngressPipeFitter : TypeBasedLinearPipeFitter
  {
    public IngressPipeFitter([NotNull] IServiceProvider serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(ExtractRelationMetadataStep);
      yield return typeof(ExtractMessageTypeStep);
      yield return typeof(DeserializeMessageStep);
      yield return typeof(ExtractAuthorizationMetadataStep);
      yield return typeof(FindMessageHandlersStep);
      yield return typeof(ExecuteMessageHandlersStep);
    }
  }
}
