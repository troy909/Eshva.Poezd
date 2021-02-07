#region Usings

using Eshva.Common.Collections;

#endregion


namespace Venture.Common.Application.MessageHandling
{
  public interface IVentureContext : IPocket
  {
    /// <summary>
    /// Commits/acknowledge the message as successfully processed to the message broker.
    /// </summary>
    void Commit(); // TODO: May be async?

    /// <summary>
    /// Marks the message as failed to process to the message broker.
    /// </summary>
    void Abort(); // TODO: May be async?
  }
}
