#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core.UnitTests.TestSubjects
{
  public interface ICustomHandler<in TMessage>
  {
    Task Handle(TMessage message, CustomMessageHandlingContext context);
  }
}
