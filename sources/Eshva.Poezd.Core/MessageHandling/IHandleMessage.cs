#region Usings

using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Common.Collections;

#endregion


namespace Eshva.Poezd.Core.MessageHandling
{
  public interface IHandleMessage
  {
    Task Handle(object message, IPocket poezdContext);
  }
}
