#region Usings

using System.Threading.Tasks;

#endregion


namespace Eshva.Poezd.Core
{
  public sealed class MessageRouter : IMessageRouter
  {
    public MessageRouter(IMessageHandlerRegistry messageHandlerRegistry, ISerializerRegistry serializerRegistry)
    {
      _messageHandlerRegistry = messageHandlerRegistry;
      _serializerRegistry = serializerRegistry;
    }

    public async Task RouteMessage(TransportMessage transportMessage)
    {
      var serializer = _serializerRegistry.GetSerializerFor(transportMessage);
      var message = serializer.Deserialize(transportMessage);
      var handlers = _messageHandlerRegistry.GetHandlersFor(message);
      foreach (var handler in handlers)
      {
        await handler.Handle(message);
      }
    }

    private readonly IMessageHandlerRegistry _messageHandlerRegistry;
    private readonly ISerializerRegistry _serializerRegistry;
  }
}
