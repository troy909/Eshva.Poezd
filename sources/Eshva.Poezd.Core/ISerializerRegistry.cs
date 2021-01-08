namespace Eshva.Poezd.Core
{
  public interface ISerializerRegistry
  {
    IMessageSerializer GetSerializerFor(TransportMessage message);
  }
}