#region Usings

using Eshva.Poezd.Core.Activation;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public class MessageHandling
  {
    public IMessageHandlersFactory MessageHandlersFactory { get; set; }
  }
}
