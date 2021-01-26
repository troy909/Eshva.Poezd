#region Usings

using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public static class MessageRouterConfigurationExtensions
  {
    public static IMessageRouter CreateMessageRouter(
      this MessageRouterConfiguration configuration,
      IDiContainerAdapter diContainerAdapter) =>
      new MessageRouter(configuration, diContainerAdapter);
  }
}
