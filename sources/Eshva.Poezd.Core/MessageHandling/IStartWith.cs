namespace Eshva.Poezd.Core.MessageHandling
{
  public interface IStartWith<in TMessage> : IHandleMessage<TMessage> where TMessage : class
  {
  }
}
