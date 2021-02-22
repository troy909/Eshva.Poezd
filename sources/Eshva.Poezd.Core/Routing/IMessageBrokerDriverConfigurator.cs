using JetBrains.Annotations;

namespace Eshva.Poezd.Core.Routing
{
  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  public interface IMessageBrokerDriverConfigurator<in TDriver> where TDriver : IMessageBrokerDriver
  {
    void Configure(TDriver driver);
  }
}
