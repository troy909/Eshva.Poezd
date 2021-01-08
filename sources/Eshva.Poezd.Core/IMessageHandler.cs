using System.Threading.Tasks;


namespace Eshva.Poezd.Core
{
  public interface IMessageHandler
  {
    Task Handle(object message);
  }
}