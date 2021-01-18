#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public static class MessageRouterConfigurationExtensions
  {
    public static IMessageRouter CreateMessageRouter(
      this MessageRouterConfiguration configuration,
      IServiceProvider serviceProvider) =>
      new MessageRouter(configuration, serviceProvider);
  }
}
