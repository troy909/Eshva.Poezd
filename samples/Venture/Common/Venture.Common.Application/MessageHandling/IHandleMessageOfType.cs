#region Usings

using System.Threading.Tasks;
using JetBrains.Annotations;

#endregion

namespace Venture.Common.Application.MessageHandling
{
  /// <summary>
  /// Marker interface for a general message handler in the application.
  /// </summary>
  /// <typeparam name="TMessage"></typeparam>
  [UsedImplicitly(ImplicitUseTargetFlags.Itself | ImplicitUseTargetFlags.WithInheritors)]
  public interface IHandleMessageOfType<in TMessage>
  {
    /// <summary>
    /// Handles a message of <typeparamref name="TMessage" /> type.
    /// </summary>
    /// <remarks>
    /// The same message object contained in the <paramref name="context" /> with the key
    /// <see cref="VentureContext.Keys.Message" />.
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
    Task Handle([NotNull] TMessage message, [NotNull] VentureContext context);
  }
}
