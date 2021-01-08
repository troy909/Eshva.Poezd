namespace Eshva.Poezd.Core
{
  public interface IMessageSerializer
  {
    TMessage Serialize<TMessage>(object message);

    object Deserialize<TMessage>(TMessage message);
  }
}