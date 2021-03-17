#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Venture.Common.Poezd.Adapter.Ingress
{
  [UsedImplicitly]
  public class VentureConsumerConfigurator : IConsumerConfigurator
  {
    public VentureConsumerConfigurator([NotNull] ILogger<VentureConsumerConfigurator> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public ConsumerBuilder<TKey, TValue> Configure<TKey, TValue>(
      ConsumerBuilder<TKey, TValue> builder,
      IDeserializer<TKey> keyDeserializer,
      IDeserializer<TValue> valueDeserializer)
    {
      builder
        .SetKeyDeserializer(keyDeserializer)
        .SetValueDeserializer(valueDeserializer)
        .SetLogHandler(LogHandler)
        .SetErrorHandler(ErrorHandler)
        .SetStatisticsHandler(StatisticsHandler)
        .SetPartitionsAssignedHandler(PartitionsAssignedHandler)
        .SetPartitionsRevokedHandler(PartitionsRevokedHandler);
      return builder;
    }

    private void PartitionsRevokedHandler<TKey, TValue>(IConsumer<TKey, TValue> consumer, List<TopicPartitionOffset> partitionOffsets)
    {
      _logger.LogDebug(
        "Revoked partitions: [@{Partitions}]",
        string.Join(", ", partitionOffsets.Select(offset => offset.Partition)));
    }

    private void PartitionsAssignedHandler<TKey, TValue>(IConsumer<TKey, TValue> consumer, List<TopicPartition> partitions)
    {
      _logger.LogDebug(
        "Assigned partitions: [@{Partitions}]",
        string.Join(", ", partitions.Select(partition => partition.Partition)));
    }

    private void StatisticsHandler<TKey, TValue>(IConsumer<TKey, TValue> consumer, string statistics)
    {
      _logger.LogInformation("Consumer statistics: @{Statistics}", statistics);
    }

    private void LogHandler<TKey, TValue>(IConsumer<TKey, TValue> consumer, LogMessage logMessage)
    {
      if (logMessage.Message.Contains("MessageSet size 0, error \"Success\"")) return;

      _logger.LogDebug(
        "Consuming from Kafka. Client: {Client}, syslog level: {LogLevel}, message: {Message}.",
        logMessage.Name,
        logMessage.Level,
        logMessage.Message);
    }

    private void ErrorHandler<TKey, TValue>(IConsumer<TKey, TValue> consumer, Error error)
    {
      if (!error.IsFatal)
        _logger.LogWarning("Consumer error: {Error}. No action required.", error);
      else
      {
        var values = consumer.Assignment;
        _logger.LogError(
          "Fatal error consuming from Kafka. Topic/partition/offset: {Topic}/{Partition}/{Offset}. Error: @{Error}.",
          string.Join(",", values.Select(a => a.Topic)),
          string.Join(",", values.Select(a => a.Partition)),
          string.Join(",", values.Select(consumer.Position)),
          error.Reason);
        throw new KafkaException(error);
      }
    }

    private readonly ILogger<VentureConsumerConfigurator> _logger;
  }
}
