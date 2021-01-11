#region Usings

#endregion

#region Usings


#endregion

using Eshva.Common;


namespace Venture.Common.Application.MessageHandling
{
  public interface IMessageHandlingContext : IPocket
  {
    void Commit(); // TODO: May be async?

    void Abort(); // TODO: May be async?
  }
}
