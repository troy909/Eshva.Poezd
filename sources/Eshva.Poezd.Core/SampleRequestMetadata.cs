using System;


namespace Eshva.Poezd.Core
{
  public class SampleRequestMetadata
  {
    public string CorrelationId { get; } // TODO: Какой на самом деле это тип?

    public string CausationId { get; }

    public DateTimeOffset SentOnUtc { get; }

    public DateTimeOffset ReceivedOnUtc { get; }
  }
}