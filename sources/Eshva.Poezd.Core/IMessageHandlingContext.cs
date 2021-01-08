#region Usings

#endregion

namespace Eshva.Poezd.Core
{
  public interface IMessageHandlingContext : IPocket
  {
    void Commit(); // TODO: May be async?

    void Abort(); // TODO: May be async?
  }
}
