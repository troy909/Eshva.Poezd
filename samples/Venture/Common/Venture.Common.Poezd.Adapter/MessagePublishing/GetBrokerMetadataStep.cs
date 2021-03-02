#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessagePublishing
{
  /// <summary>
  /// An egress message pipeline step that gets broker message metadata that specific for public API and store it into
  /// context.
  /// </summary>
  public class GetBrokerMetadataStep : IStep<MessagePublishingContext>
  {
    /// <inheritdoc />
    public Task Execute(MessagePublishingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      context.Metadata = new Dictionary<string, string>
      {
        {
          VentureApi.Headers.MessageTypeName,
          context.PublicApi.MessageTypesRegistry.GetMessageTypeNameByItsMessageType(context.Message.GetType())
        },
        {VentureApi.Headers.CorrelationId, context.CorrelationId},
        {VentureApi.Headers.CausationId, context.CausationId},
        {VentureApi.Headers.MessageId, context.MessageId}
      };

      return Task.CompletedTask;
    }
  }
}
