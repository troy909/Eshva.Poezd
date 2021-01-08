namespace Eshva.Poezd.Core
{
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
}