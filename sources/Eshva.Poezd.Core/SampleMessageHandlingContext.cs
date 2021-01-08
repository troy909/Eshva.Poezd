namespace Eshva.Poezd.Core
{
  public class SampleMessageHandlingContext : IMessageHandlingContext<SampleMessageMetadata, SampleRequestMetadata>
  {
    public SampleMessageMetadata Message { get; }

    public SampleRequestMetadata Request { get; }

    public void Commit()
    {
    }

    public void Abort()
    {
    }
  }
}
