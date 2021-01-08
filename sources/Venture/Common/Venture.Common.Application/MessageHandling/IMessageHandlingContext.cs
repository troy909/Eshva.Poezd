#region Usings

#endregion

#region Usings

using Venture.Common.Tools;

#endregion


namespace Venture.Common.Application.MessageHandling
{
  public interface IMessageHandlingContext : IPocket
  {
    void Commit(); // TODO: May be async?

    void Abort(); // TODO: May be async?
  }
}
