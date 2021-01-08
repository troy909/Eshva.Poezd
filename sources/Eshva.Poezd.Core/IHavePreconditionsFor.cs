#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core
{
  public interface IHavePreconditionsFor<in TMessage> where TMessage : class
  {
    public Task<bool> ShouldHandle(TMessage message, VentureMessageHandlingContext context);
  }
}
