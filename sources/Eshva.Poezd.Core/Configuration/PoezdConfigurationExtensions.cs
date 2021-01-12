#region Usings

using Eshva.Poezd.Core.Routing;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public static class PoezdConfigurationExtensions
  {
    public static IMessageRouter BuildMessageRouter(this PoezdConfiguration configuration) => new DefaultMessageRouter(configuration);
  }
}
