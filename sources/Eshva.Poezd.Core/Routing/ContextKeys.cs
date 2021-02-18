#region Usings

using System;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Keys of items in broker message handling context.
  /// </summary>
  /// <remarks>
  /// You shouldn't use this class and make reference to it from your message handlers from application layer. The only place
  /// where you can use it is broker message handling steps which should be placed in your Poezd to service adapter.
  /// If you need anything from the broker message handling context in your application message handler you should create
  /// your own message handling context in application layer and copy required items from broker message handling context
  /// into it.
  /// </remarks>
  public static class ContextKeys
  {
    public static class Broker
    {
      /// <summary>
      /// The key of message payload as received from message broker.
      /// </summary>
      /// <value>
      /// The value is a <see cref="byte[]" />.
      /// </value>
      public const string MessagePayload = Prefix + "message-payload";

      /// <summary>
      /// The key of message metadata as received from message broker.
      /// </summary>
      /// <value>
      /// The value is a IDictionary&lt;string, string&gt; />.
      /// </value>
      public const string MessageMetadata = Prefix + "message-metadata";

      /// <summary>
      /// Broker ID from configuration.
      /// </summary>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      public const string Id = Prefix + "id";

      /// <summary>
      /// The name of the queue/topic the message in this context came from.
      /// </summary>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      public const string QueueName = Prefix + "queue-name";

      /// <summary>
      /// Date/time the message in this context received.
      /// </summary>
      /// <value>
      /// The value is a <see cref="System.DateTimeOffset" />.
      /// </value>
      public const string ReceivedOnUtc = Prefix + "received-on-utc";

      /// <summary>
      /// The broker configuration.
      /// </summary>
      /// <value>
      /// The value is a <see cref="Eshva.Poezd.Core.Configuration.MessageBrokerConfiguration" />.
      /// </value>
      public const string Configuration = Prefix + "configuration";

      private const string Prefix = "broker-";
    }

    public static class PublicApi
    {
      /// <summary>
      /// Public API ID from configuration.
      /// </summary>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      public const string Id = Prefix + "id";

      /// <summary>
      /// The configuration of public API.
      /// </summary>
      /// <value>
      /// The value is a <see cref="Eshva.Poezd.Core.Configuration.PublicApiConfiguration" />.
      /// </value>
      public const string Configuration = Prefix + "configuration";

      /// <summary>
      /// The message type registry of public API.
      /// </summary>
      /// <value>
      /// The value is a <see cref="Eshva.Poezd.Core.Pipeline.IMessageTypesRegistry" />.
      /// </value>
      public const string MessageTypesRegistry = Prefix + "message-types-registry";

      private const string Prefix = "public-api-";
    }

    /// <summary>
    /// Standard for this library keys of application metadata in a message handling context.
    /// </summary>
    public static class Application
    {
      /// <summary>
      /// The key of the current message as CLR-object extracted from external service message payload.
      /// </summary>
      /// <value>
      /// The value type depends on message type.
      /// </value>
      public const string MessagePayload = Prefix + "message-payload";

      /// <summary>
      /// The key of application message type.
      /// </summary>
      /// <value>
      /// The value is a <see cref="Type" />.
      /// </value>
      public const string MessageType = Prefix + "message-type";

      /// <summary>
      /// The key of application message type name.
      /// </summary>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      public const string MessageTypeName = Prefix + "message-type-name";

      /// <summary>
      /// The key of application message type descriptor used for serialization and parsing messages of this type.
      /// </summary>
      /// <value>
      /// The value is a <see cref="Eshva.Poezd.Core.Pipeline.IMessageTypeDescriptor{TMessageType}" />
      /// </value>
      public const string MessageTypeDescriptor = Prefix + "message-type-descriptor";

      /// <summary>
      /// The key of the message ID.
      /// </summary>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      public const string MessageId = Prefix + "message-id";

      /// <summary>
      /// The key of correlation ID of the first message in this message conversation.
      /// </summary>
      /// <remarks>
      /// Correlation ID allows to trace the entire conversation between communicating parties.
      /// </remarks>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      public const string CorrelationId = Prefix + "correlation-id";

      /// <summary>
      /// The key of the message ID that causes this one.
      /// </summary>
      /// <remarks>
      /// Causation ID allows to find the direct cause of this message and trace it in conversation between communicating
      /// parties.
      /// </remarks>
      /// <value>
      /// The value is a <see cref="string" />.
      /// </value>
      public const string CausationId = Prefix + "correlation-id";

      /// <summary>
      /// The key of message handler list for the current message.
      /// </summary>
      /// <value>
      /// The type of value depends on Poezd to service adapter.
      /// </value>
      public const string Handlers = Prefix + "handlers";

      private const string Prefix = "application-";
    }
  }
}
