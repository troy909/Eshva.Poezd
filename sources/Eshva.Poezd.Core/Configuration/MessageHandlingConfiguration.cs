#region Usings

using Eshva.Poezd.Core.Activation;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class MessageHandlingConfiguration
  {
    public IMessageHandlersFactory MessageHandlersFactory { get; set; }
  }
}
