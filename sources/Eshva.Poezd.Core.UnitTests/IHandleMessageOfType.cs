#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests
{
  public interface IHandleMessageOfType<in TMessage> where TMessage : class
  {
    Task Handle(TMessage message, IMessageHandlingContext context);
  }
}
