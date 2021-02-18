#region Usings

using System.Threading.Tasks;

#endregion

namespace Venture.Common.Application.MessageHandling
{
  public interface IHavePreconditionsFor<in TMessage> where TMessage : class
  {
    public Task<bool> ShouldHandle(TMessage message, VentureContext context);
  }
}
