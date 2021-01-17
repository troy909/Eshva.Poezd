#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public static class PoezdConfigurationExtensions
  {
    public static IMessageRouter CreateMessageRouter(
      this PoezdConfiguration configuration,
      IServiceProvider serviceProvider) =>
      new MessageRouter(configuration, serviceProvider);
  }
}
