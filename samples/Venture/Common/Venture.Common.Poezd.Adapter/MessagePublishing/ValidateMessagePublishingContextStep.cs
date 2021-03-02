#region Usings

using System;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.MessagePublishing
{
  /// <summary>
  /// An egress message pipeline step that validates message publishing context content before other pipeline steps will be
  /// executed.
  /// </summary>
  public class ValidateMessagePublishingContextStep : IStep<MessagePublishingContext>
  {
    /// <inheritdoc />
    public Task Execute(MessagePublishingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));
      if (context.Message == null)
        throw new ArgumentException("Message should be set in the message publishing context.", nameof(context));
      if (string.IsNullOrWhiteSpace(context.CorrelationId))
        throw new ArgumentException("Correlation ID should be set in the message publishing context.", nameof(context));
      if (string.IsNullOrWhiteSpace(context.CausationId))
        throw new ArgumentException("Causation ID should be set in the message publishing context.", nameof(context));
      if (string.IsNullOrWhiteSpace(context.MessageId))
        throw new ArgumentException("Message ID should be set in the message publishing context.", nameof(context));

      if (context.PublicApi == null)
        throw new ArgumentException("Public API should be set in the message publishing context.", nameof(context));
      if (context.PublicApi.MessageTypesRegistry == null)
      {
        throw new ArgumentException(
          "Message types registry of public API should be set in the message publishing context.",
          nameof(context));
      }

      return Task.CompletedTask;
    }
  }
}
