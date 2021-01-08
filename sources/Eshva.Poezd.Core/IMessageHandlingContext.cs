#region Usings

using System;

#endregion


namespace Eshva.Poezd.Core
{
  public interface IMessageHandlingContext
  {
    string MessageType { get; }

    string MessageId { get; }

    string Source { get; } // TODO: Заменить на тип или интерфейс, если нужно будет предоставлять больше информации об источнике.

    string CorrelationId { get; } // TODO: Какой на самом деле это тип?

    string CausationId { get; }

    DateTimeOffset SentOnUtc { get; }

    DateTimeOffset ReceivedOnUtc { get; }

    void Commit(); // TODO: May be async?

    void Uncommit(); // TODO: May be async?
  }
}
