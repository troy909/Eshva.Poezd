#region Usings

using Eshva.Common.Collections;

#endregion


namespace Venture.Common.Application.MessageHandling
{
  public interface IVentureMessageHandlingContext : IPocket
  {
    void Commit(); // TODO: May be async?

    void Abort(); // TODO: May be async?
  }
}
