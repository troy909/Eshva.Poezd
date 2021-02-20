#region Usings

using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Extensions for <see cref="MessageRouter" />.
  /// </summary>
  public static class MessageRouterConfigurationExtensions
  {
    /// <summary>
    /// Creates a message router with configuration and DI-container adapter specified.
    /// </summary>
    /// <param name="configuration">
    /// Message router configuration.
    /// </param>
    /// <param name="diContainerAdapter">
    /// DI-container adapter.
    /// </param>
    /// <returns>
    /// Configured message router.
    /// </returns>
    public static IMessageRouter CreateMessageRouter(
      this MessageRouterConfiguration configuration,
      IDiContainerAdapter diContainerAdapter) =>
      new MessageRouter(configuration, diContainerAdapter);
  }
}
