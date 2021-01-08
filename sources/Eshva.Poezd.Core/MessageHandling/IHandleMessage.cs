#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.MessageHandling
{
  public interface IHandleMessage<in TMessage> where TMessage : class
  {
    Task Handle(TMessage message, object context);
  }
}
