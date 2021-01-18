namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageBrokerDriverConfigurator<in TDriver> where TDriver : IMessageBrokerDriver
  {
    void Configure(TDriver driver);
  }
}
