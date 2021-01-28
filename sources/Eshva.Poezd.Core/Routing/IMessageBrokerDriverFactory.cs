#region Usings

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public interface IMessageBrokerDriverFactory
  {
    IMessageBrokerDriver Create();
  }
}
