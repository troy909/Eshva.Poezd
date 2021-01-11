#region Usings

using System.Threading.Tasks;
using Eshva.Common;

#endregion


namespace Eshva.Poezd.Core.MessageHandling
{
  public interface IHandleMessage<in TMessage>
  {
    Task Handle(TMessage message, IPocket poezdContext);
  }
}
