namespace Eshva.Poezd.Core
{
  public class TransportMessageMetadata
  {
    public TransportMessageMetadata(string sourceTopic, string type)
    {
      SourceTopic = sourceTopic;
      Type = type;
    }

    public string Type { get; }

    public string SourceTopic { get; }
  }
}