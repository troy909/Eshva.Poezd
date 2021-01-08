#region Usings

#endregion


namespace Eshva.Poezd.Core
{
  public interface IMessageHandlingContext<out TMessageMetadata, out TRequestMetadata>
  {
    TMessageMetadata Message { get; }

    TRequestMetadata Request { get; }

    void Commit(); // TODO: May be async?

    void Abort(); // TODO: May be async?
  }
}
