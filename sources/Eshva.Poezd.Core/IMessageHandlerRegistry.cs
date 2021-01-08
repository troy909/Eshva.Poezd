namespace Eshva.Poezd.Core
{
  public interface IMessageHandlerRegistry
  {
    IMessageHandler[] GetHandlersFor(object message);
  }
}