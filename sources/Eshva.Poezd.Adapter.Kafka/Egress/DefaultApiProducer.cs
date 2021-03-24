#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// The default Kafka API producer.
  /// </summary>
  /// <typeparam name="TKey">
  /// The message key type.
  /// </typeparam>
  /// <typeparam name="TValue">
  /// The message payload type.
  /// </typeparam>
  internal class DefaultApiProducer<TKey, TValue> : IApiProducer
  {
    /// <summary>
    /// Constructs a new instance of API producer.
    /// </summary>
    /// <param name="producer">
    /// The Kafka producer used to publish messages.
    /// </param>
    /// <param name="headerValueCodec">
    /// The codec for convert header values from string into byte array.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    public DefaultApiProducer(
      [NotNull] IProducer<TKey, TValue> producer,
      [NotNull] IHeaderValueCodec headerValueCodec,
      [NotNull] ILogger<DefaultApiProducer<TKey, TValue>> logger)
    {
      _producer = producer ?? throw new ArgumentNullException(nameof(producer));
      _headerValueCodec = headerValueCodec ?? throw new ArgumentNullException(nameof(headerValueCodec));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken)
    {
      if (context == null) throw new ArgumentNullException(nameof(context));

      var errors = ValidateContext(context);
      if (!string.IsNullOrWhiteSpace(errors)) throw new PoezdOperationException($"Message publishing context has missing data: {errors}.");

      var headers = MakeHeaders(context.Metadata);
      var timestamp = new Timestamp(context.Timestamp);
      var brokerId = context.Broker.Id;
      var key = (TKey) context.Key;
      var payload = (TValue) context.Payload;
      var message = new Message<TKey, TValue> {Key = key, Value = payload, Headers = headers, Timestamp = timestamp};
      var publishTasks = context.QueueNames.Select(
        async queueName =>
        {
          try
          {
            _logger.LogDebug(
              "Publishing a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
              key,
              queueName,
              brokerId);
            await _producer.ProduceAsync(
              queueName,
              message,
              cancellationToken);
            _logger.LogDebug(
              "Successfully published a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
              key,
              queueName,
              brokerId);
          }
          catch (Exception exception)
          {
            _logger.LogDebug(
              exception,
              "Failed to publish a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
              key,
              queueName,
              brokerId);
            throw;
          }
        });

      return Task.WhenAll(publishTasks);
    }

    /// <inheritdoc />
    public void Dispose() => _producer.Dispose();

    private static string ValidateContext(MessagePublishingContext context)
    {
      var errors = new List<string>();
      if (context.Metadata == null) errors.Add(nameof(context.Metadata));
      if (context.Payload == null) errors.Add(nameof(context.Payload));
      if (context.Key == null) errors.Add(nameof(context.Key));
      if (context.Api == null) errors.Add(nameof(context.Api));
      if (context.QueueNames == null) errors.Add(nameof(context.QueueNames));
      if (context.Broker == null) errors.Add(nameof(context.Broker));

      return string.Join(", ", errors);
    }

    private Headers MakeHeaders(IReadOnlyDictionary<string, string> metadata)
    {
      var headers = new Headers();
      foreach (var (key, value) in metadata)
      {
        headers.Add(key, _headerValueCodec.Encode(value));
      }

      return headers;
    }

    private readonly IHeaderValueCodec _headerValueCodec;
    private readonly ILogger<DefaultApiProducer<TKey, TValue>> _logger;
    private readonly IProducer<TKey, TValue> _producer;
  }
}
