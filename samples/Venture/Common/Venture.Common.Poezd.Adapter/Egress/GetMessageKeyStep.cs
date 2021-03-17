#region Usings

using System;
using System.Reflection;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Venture.Common.Poezd.Adapter.Egress
{
  /// <summary>
  /// An egress message pipeline step that gets key of message and stores it into context.
  /// </summary>
  public class GetMessageKeyStep : IStep<MessagePublishingContext>
  {
    /// <inheritdoc />
    public Task Execute(MessagePublishingContext context)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var message = context.Message;
      var registry = context.Api.MessageTypesRegistry;
      var getKey = GenericGetKey!.MakeGenericMethod(message.GetType());
      context.Key = (byte[]) getKey.Invoke(this, new[] {message, registry});
      return Task.CompletedTask;
    }

    private object GetKey<TMessage>(TMessage message, IEgressMessageTypesRegistry registry) where TMessage : class =>
      registry.GetDescriptorByMessageType<TMessage>().GetKey(message);

    private static readonly MethodInfo GenericGetKey =
      typeof(GetMessageKeyStep).GetMethod(nameof(GetKey), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
