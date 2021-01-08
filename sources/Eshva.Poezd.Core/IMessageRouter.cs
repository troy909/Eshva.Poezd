using System.Threading.Tasks;


namespace Eshva.Poezd.Core
{
  public interface IMessageRouter
  {
    Task RouteMessage(TransportMessage transportMessage);
  }
}