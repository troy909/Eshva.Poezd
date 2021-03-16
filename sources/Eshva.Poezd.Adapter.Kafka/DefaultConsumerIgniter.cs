#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  internal class DefaultConsumerIgniter<TKey, TValue> : IConsumerIgniter<TKey, TValue>
  {
    public DefaultConsumerIgniter([NotNull] ILogger<BrokerIngressKafkaDriver> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Start(
      IConsumer<TKey, TValue> consumer,
      Func<ConsumeResult<TKey, TValue>, Task> onMessageReceived,
      CancellationToken cancellationToken)
    {
      return Task.Factory.StartNew(
        async () =>
        {
          try
          {
            while (!cancellationToken.IsCancellationRequested)
            {
              var consumeResult = consumer.Consume(cancellationToken);
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
              consumer.Commit(consumeResult);
            }
          }
          catch (KafkaException exception)
          {
            _logger.LogError(
              exception,
              "A Kafka error occurred in consuming loop: {Error}",
              exception.Error);
            throw;
          }
          catch (Exception exception)
          {
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
    }

    private readonly ILogger<BrokerIngressKafkaDriver> _logger;
  }
}
