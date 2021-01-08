#region Usings

using System;

#endregion


namespace Eshva.Poezd.Core
{
  public class TestMessageHandlingContext : IMessageHandlingContext
  {
    public string MessageType { get; }

    public string MessageId { get; }

    public string Source { get; }

    public string CorrelationId { get; }

    public string CausationId { get; }

    public DateTimeOffset SentOnUtc { get; }

    public DateTimeOffset ReceivedOnUtc { get; }

    public void Commit()
    {
    }

    public void Uncommit()
    {
    }
  }
}
