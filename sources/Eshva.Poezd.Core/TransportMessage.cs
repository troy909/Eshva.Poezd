namespace Eshva.Poezd.Core
{
  public class TransportMessage
  {
    public TransportMessage(TransportMessageMetadata metadata, byte[] data)
    {
      Metadata = metadata;
      Data = data;
    }

    public TransportMessageMetadata Metadata { get; }

    public byte[] Data { get; }
  }
}