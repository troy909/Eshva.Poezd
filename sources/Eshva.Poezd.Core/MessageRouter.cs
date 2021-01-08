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

  public interface IMessageRouter
  {
    Task RouteMessage(TransportMessage transportMessage);
  }

  public class KafkaConnector : IMessageBrokerConnector
  {
    public KafkaConnector(IMessageRouter messageRouter)
    {
      _messageRouter = messageRouter;
    }

    private void HandleIncomingMessage(object kafkaMessage)
    {
      var transportMessage = new TransportMessage(
        new TransportMessageMetadata("kafka topic", "message type from headers"),
        new byte[0]
      );

      _messageRouter.RouteMessage(transportMessage);
    }

    private readonly IMessageRouter _messageRouter;
  }

  public interface IMessageBrokerConnector
  {
  }

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

  public interface IMessageHandlerRegistry
  {
    IMessageHandler[] GetHandlersFor(object message);
  }

  public interface ISerializerRegistry
  {
    IMessageSerializer GetSerializerFor(TransportMessage message);
  }

  public interface IMessageSerializer
  {
    TMessage Serialize<TMessage>(object message);

    object Deserialize<TMessage>(TMessage message);
  }

  public interface IMessageHandler
  {
    Task Handle(object message);
  }
}
