#region Usings

using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.MessageHandling
{
  /// <summary>
  /// Contract of the message publisher for Venture product.
  /// </summary>
  /// <remarks>
  /// You should have a similar service declared in the application level. Its implementation should be defined in the
  /// product Poezd adapter.
  /// </remarks>
  public interface IMessagePublisher
  {
    /// <summary>
    /// Publishes message to the outer world.
    /// </summary>
    /// <typeparam name="TMessage">
    /// The type of the publishing message.
    /// </typeparam>
    /// <param name="message">
    /// The message to publish.
    /// </param>
    /// <param name="context">
    /// The message handling context. It should contain the following items:
    /// * Correlation ID
    /// * Causation ID
    /// * Message ID. This one could be missed, in which case it will be generated from a new GUID.
    /// </param>
    /// <returns>
    /// The task that could be used to wait when publish is finished.
    /// </returns>
    [NotNull]
    Task Publish<TMessage>([NotNull] TMessage message, [NotNull] VentureContext context) where TMessage : class;
  }
}
