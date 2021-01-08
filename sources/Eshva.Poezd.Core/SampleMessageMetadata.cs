namespace Eshva.Poezd.Core
{
  public class SampleMessageMetadata
  {
    public string Type { get; }

    public string Id { get; }

    public string Source { get; } // TODO: Заменить на тип или интерфейс, если нужно будет предоставлять больше информации об источнике.
  }
}