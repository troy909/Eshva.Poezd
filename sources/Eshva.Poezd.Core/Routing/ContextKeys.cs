namespace Eshva.Poezd.Core.Routing
{
  public static class ContextKeys
  {
    public static class Broker
    {
      /// <summary>
      /// The key of message payload as received from message broker. The value is byte[].
      /// </summary>
      public const string MessagePayload = "broker-message-payload";

      /// <summary>
      /// The key of message metadata as received from message broker. The value is IDictionary&lt;string, string&gt;.
      /// </summary>
      public const string MessageMetadata = "broker-message-metadata";

      public const string MessageId = "broker-message-id";
      public const string Id = "broker-id";
      public const string QueueName = "broker-queue-name";
      public const string ReceivedOnUtc = "broker-received-on-utc";
      public const string Configuration = "broker-configuration";
    }

    public static class PublicApi
    {
      /// <summary>
      /// The key of external service message payload extracted from message broker payload. The value is byte[].
      /// </summary>
      public const string MessagePayload = "external-service-message-payload";

      /// <summary>
      /// The key of external service message metadata extracted from broker message payload.
      /// The value is IDictionary&lt;string, strung&gt;.
      /// </summary>
      public const string MessageMetadata = "external-service-message-metadata";

      public const string MessageId = "external-service-message-id";

      public const string Id = "external-service-id";

      public const string Configuration = "external-service-configuration";
    }

    /// <summary>
    /// Standard for this library keys of application metadata in a message handling context.
    /// </summary>
    public static class Application
    {
      /// <summary>
      /// The key of the current message as CLR-object extracted from external service message payload.
      /// The value type depends on message type.
      /// </summary>
      public const string MessagePayload = Prefix + "message-payload";

      /// <summary>
      /// The key of application message type. The value is a <see cref="Type" />.
      /// </summary>
      public const string Type = Prefix + "message-type";

      /// <summary>
      /// The key of application message type name. The value is a <see cref="string" />.
      /// </summary>
      public const string TypeName = Prefix + "message-type-name";

      /// <summary>
      /// The key of the message ID. The value is a <see cref="string" />.
      /// </summary>
      public const string MessageId = Prefix + "message-id";

      /// <summary>
      /// The key of correlation ID of the first message in this message conversation. The value is a <see cref="string" />.
      /// </summary>
      /// <remarks>
      /// Correlation ID allows to trace the entire conversation between communicating parties.
      /// </remarks>
      public const string CorrelationId = Prefix + "correlation-id";

      /// <summary>
      /// The key of the message ID that causes this one. The value is a <see cref="string" />.
      /// </summary>
      /// <remarks>
      /// Causation ID allows to find the direct cause of this message and trace it in conversation between communicating
      /// parties.
      /// </remarks>
      public const string CausationId = Prefix + "correlation-id";

      private const string Prefix = "application-";
    }

    public static class MessageHandling
    {
      /// <summary>
      /// The key of message handler list for the current message. The value is IEnumerable&lt;IHandleMessage&gt;"/>.
      /// </summary>
      public const string Handlers = "message-handling-handlers";
    }
  }
}
