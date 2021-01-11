#region Usings

using Eshva.Poezd.Core.Routing;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public class MessageRouter : IMessageRouter
  {
    public MessageRouter(PoezdConfiguration configuration)
    {
      _configuration = configuration;
    }

    private readonly PoezdConfiguration _configuration;
  }
}
