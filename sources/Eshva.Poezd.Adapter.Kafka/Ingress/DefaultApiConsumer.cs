#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// Default API consumer.
  /// </summary>
  /// <typeparam name="TKey">
  /// The message key type.
  /// </typeparam>
  /// <typeparam name="TValue">
  /// The message payload type.
  /// </typeparam>
  internal class DefaultApiConsumer<TKey, TValue> : IApiConsumer<TKey, TValue>
  {
    /// <summary>
    /// Constructs a new instance of default API consumer.
    /// </summary>
    /// <param name="api">
    /// The ingress API which messages this consumer process.
    /// </param>
    /// <param name="consumer">
    /// The Kafka API consumer.
    /// </param>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public DefaultApiConsumer(
      [NotNull] IIngressApi api,
      [NotNull] IConsumer<TKey, TValue> consumer,
      [NotNull] ILogger<DefaultApiConsumer<TKey, TValue>> logger)
    {
      _api = api ?? throw new ArgumentNullException(nameof(api));
      _consumer = consumer ?? throw new ArgumentNullException(nameof(consumer));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public Task Start(Func<ConsumeResult<TKey, TValue>, Task> onMessageReceived, CancellationToken cancellationToken)
    {
      SubscribeToTopics();

      var result = Task.Factory.StartNew(
        async () =>
        {
          try
          {
            while (!cancellationToken.IsCancellationRequested)
            {
              var consumeResult = _consumer.Consume(cancellationToken);
              if (consumeResult.IsPartitionEOF) continue;

              await onMessageReceived(consumeResult);
              _logger.LogDebug(
                "Received message at {Offset}: {Value}",
                consumeResult.TopicPartitionOffset,
                consumeResult.Message.Value);

              // TODO: Choose how to commit received messages.
              // if (consumeResult.Offset.Value % _configuration.CommitPeriod == 0)
              // The Commit method sends a "commit offsets" request to the Kafka
              // cluster and synchronously waits for the response. This is very
              // slow compared to the rate at which the consumer is capable of
              // consuming messages. A high performance application will typically
              // commit offsets relatively infrequently and be designed handle
              // duplicate messages in the event of failure.
              _consumer.Commit(consumeResult);
            }
          }
          catch (KafkaException exception)
          {
            // TODO: Handle Kafka errors related to loosing connection to server.
            _logger.LogError(
              exception,
              "A Kafka error occurred in consuming loop: {Error}",
              exception.Error);
            throw;
          }
          catch (Exception exception)
          {
            // TODO: What should I do with other errors?
            // TODO: Should I handle errors with some external strategy kept in ingress API.
            _logger.LogError(
              exception,
              "An error occurred in Kafka consuming loop: {Error}",
              exception.Message);
            throw;
          }
        },
        cancellationToken,
        TaskCreationOptions.LongRunning,
        TaskScheduler.Default
      );

      _started = true;
      return result;
    }

    /// <inheritdoc />
    public void Stop()
    {
      if (!_started || _stopped) return;

      try
      {
        _consumer.Close();
      }
      catch (Exception exception)
      {
        // Mostly ignore.
        // TODO: Isn't it a problem that not every message will be committed?
        _logger.LogInformation("During the final offsets commit an error occurred: @{Exception}", exception);
      }
      finally
      {
        _stopped = true;
      }

      _logger.LogInformation(
        "Closed consumer with name {Name} and {GroupID}.",
        _consumer.Name,
        _consumer.MemberId);
    }

    /// <inheritdoc />
    public void Dispose()
    {
      Stop();
      _consumer.Dispose();
    }

    private void SubscribeToTopics()
    {
      // TODO: Handle exception during subscribing.
      _consumer.Subscribe(_api.GetQueueNamePatterns());
    }

    private readonly IIngressApi _api;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly ILogger<DefaultApiConsumer<TKey, TValue>> _logger;
    private bool _started;
    private bool _stopped;
  }
}
