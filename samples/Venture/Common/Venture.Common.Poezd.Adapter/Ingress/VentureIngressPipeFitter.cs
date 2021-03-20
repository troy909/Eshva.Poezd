#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Poezd.Adapter.Ingress
{
  /// <summary>
  /// A Venture service ingress message handling pipe fitter.
  /// </summary>
  /// <remarks>
  /// A Venture service contract:
  /// <list type="bullet">
  /// <item>
  /// <descriptin>uses Kafka to send/receive messages;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>publishes events of its aggregates to topics named <c>case.facts.[aggregate-name].v1</c>;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>sends message type in a broker header named <c>type</c>;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>serializes its messages using FlatBuffers;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>sends message ID in a broker header named <c>id</c>;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>sends correlation ID in a broker header named <c>correlation-id</c>;</descriptin>
  /// </item>
  /// <item>
  /// <descriptin>sends causation ID in a broker header named <c>causation-id</c>;</descriptin>
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
  public sealed class VentureIngressPipeFitter : TypeBasedPipeFitter
  {
    public VentureIngressPipeFitter([NotNull] IDiContainerAdapter serviceProvider) : base(serviceProvider) { }

    protected override IEnumerable<Type> GetStepTypes()
    {
      yield return typeof(ExtractRelationMetadataStep);
      yield return typeof(ExtractMessageTypeStep);
      yield return typeof(ParseBrokerMessageStep);
      yield return typeof(FindMessageHandlersStep);
      yield return typeof(ExecuteMessageHandlersStep);
    }
  }
}
