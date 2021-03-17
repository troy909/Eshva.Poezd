#region Usings

using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.Ingress
{
  /// <summary>
  /// Marker interface for a general message handler in the application.
  /// </summary>
  /// <remarks>
  /// Each handler class can implement this interface many types for every handled message type.
  /// </remarks>
  /// <typeparam name="TMessage">
  /// The type of handling message.
  /// </typeparam>
  [UsedImplicitly]
  public interface IMessageHandler<in TMessage>
  {
    /// <summary>
    /// Handles a message of <typeparamref name="TMessage" /> type.
    /// </summary>
    /// <remarks>
    /// The same message object contained within the <paramref name="context" />.
    /// </remarks>
    /// <param name="message">
    /// The message object.
    /// </param>
    /// <param name="context">
    /// The message handling context containing everything to successfully handling the message.
    /// </param>
    /// <returns>
    /// A task object that can be used to wait for message to be handled.
    /// </returns>
    // ReSharper disable once UnusedParameter.Global - could be used in a real application.
    Task Handle([NotNull] TMessage message, [NotNull] VentureIncomingMessageHandlingContext context);
  }
}
