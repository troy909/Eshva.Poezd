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

namespace Eshva.Poezd.KafkaCoupling
{
  public class BrokerIngressKafkaDriver : IBrokerIngressDriver
  {
    public BrokerIngressKafkaDriver([NotNull] BrokerIngressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public void Dispose()
    {
      try
      {
        _consumer?.Commit();
      }
      catch (Exception exception)
      {
        // Mostly ignore.
        _logger.LogInformation("During the final offsets commit an error occurred: @{Exception}", exception);
      }

      _consumer?.Close();
      _logger.LogInformation(
        "Closed consumer with bootstrap servers '@{BootstrapServers}' and @{GroupID}.",
        _configuration.ConsumerConfig.BootstrapServers,
        _configuration.ConsumerConfig.GroupId);

      _consumer?.Dispose();
    }

    /// <inheritdoc />
    public void Initialize(
      IMessageRouter messageRouter,
      string brokerId,
      IServiceProvider serviceProvider)
    {
      _messageRouter = messageRouter ?? throw new ArgumentNullException(nameof(messageRouter));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _logger = (ILogger<BrokerIngressKafkaDriver>) serviceProvider.GetService(typeof(ILogger<BrokerIngressKafkaDriver>)) ??
                throw new PoezdConfigurationException(
                  $"Can not get a logger of type {typeof(ILogger<BrokerIngressKafkaDriver>).FullName}.");

      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));
      _brokerId = brokerId;

      if (_isInitialized)
        throw new PoezdOperationException($"Kafka driver for broker with ID '{_brokerId}' is already initialized.");

      _consumer = CreateConsumer();

      _isInitialized = true;
    }

    /// <inheritdoc />
    public Task StartConsumeMessages(
      IEnumerable<string> queueNamePatterns,
      CancellationToken cancellationToken = default)
    {
      if (!_isInitialized) throw new PoezdOperationException("Kafka driver should be initialized before it can consume messages.");

      if (queueNamePatterns == null) throw new ArgumentNullException(nameof(queueNamePatterns));
      var patterns = queueNamePatterns.ToArray();
      if (!patterns.Any()) throw new ArgumentException("List of patterns contains no patterns. It should contain at least one pattern.");

      _consumer.Subscribe(patterns);

      Consume(OnMessageReceived, cancellationToken);
      return Task.CompletedTask;
    }

    private async Task OnMessageReceived(ConsumeResult<Ignore, byte[]> consumeResult)
    {
      // TODO: handle in parallel? No we need a strategy.
      if (!(_serviceProvider.GetService(_configuration.HeaderValueParserType) is IHeaderValueParser parser))
      {
        throw new PoezdOperationException(
          "Can not parse Kafka broker message headers because can not get a header value parser. " +
          $"You should register a service of type {_configuration.HeaderValueParserType.FullName} in your DI-container.");
      }

      var headers = consumeResult.Message.Headers.ToDictionary(
        header => header.Key,
        header => parser.Parser(header.GetValueBytes()));
      await _messageRouter.RouteIngressMessage(
        _brokerId,
        consumeResult.Topic,
        consumeResult.Message.Timestamp.UtcDateTime,
        consumeResult.Message.Value,
        headers);
    }

    private void Consume(Func<ConsumeResult<Ignore, byte[]>, Task> onMessageReceived, CancellationToken cancellationToken = default)
    {
      _consumeTask = Task.Factory.StartNew(
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
                "Received message at @{Offset}: @{Value}",
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
          catch (ConsumeException exception)
          {
            _logger.LogError(
              exception,
              "A Kafka consume error occurred: {Error}",
              exception.Error);
          }
          finally
          {
            _logger.LogInformation("Closing Kafka consumer.");
            _consumer.Close();
            _logger.LogInformation("Kafka consumer closed.");
          }
        },
        cancellationToken,
        TaskCreationOptions.LongRunning,
        TaskScheduler.Default
      );
    }

    private IConsumer<Ignore, byte[]> CreateConsumer()
    {
      try
      {
        _logger.LogInformation("Creating Kafka consumer with configuration @{ConsumerConfig}", _configuration.ConsumerConfig);
        var consumer = new ConsumerBuilder<Ignore, byte[]>(_configuration.ConsumerConfig)
          .SetKeyDeserializer(Deserializers.Ignore)
          .SetLogHandler(ConsumerOnLog)
          .SetErrorHandler(ConsumerOnError)
          .SetStatisticsHandler(
            (_, statistics) => _logger.LogInformation(
              "Consumer statistics: @{Statistics}",
              statistics))
          .SetPartitionsAssignedHandler(
            (_, partitions) => _logger.LogDebug(
              "Assigned partitions: [@{Partitions}]",
              string.Join(", ", partitions.Select(partition => partition.Partition))))
          .SetPartitionsRevokedHandler(
            (_, partitionOffsets) => _logger.LogDebug(
              "Revoked partitions: [@{Partitions}]",
              string.Join(", ", partitionOffsets.Select(offset => offset.Partition))))
          .Build();
        _logger.LogInformation("A Kafka consumer created.");
        return consumer;
      }
      catch (Exception exception)
      {
        const string errorMessage = "During Kafka consumer creating an error occurred.";
        _logger.LogError(exception, errorMessage);
        throw new PoezdOperationException(errorMessage, exception);
      }
    }

    private void ConsumerOnLog(IConsumer<Ignore, byte[]> arg1, LogMessage logMessage)
    {
      // To skip messages about empty reads.
      if (logMessage.Message.Contains("MessageSet size 0, error \"Success\"")) return;

      _logger.LogDebug(
        "Consuming from Kafka. Client: '@{Client}', syslog level: '@{LogLevel}', message: '@{Message}'.",
        logMessage.Name,
        logMessage.Level,
        logMessage.Message);
    }

    private void ConsumerOnError(IConsumer<Ignore, byte[]> consumer, Error error)
    {
      if (!error.IsFatal)
        _logger.LogWarning("Consumer error: @{Error}. No action required.", error);
      else
      {
        var values = consumer.Assignment;
        _logger.LogError(
          "Fatal error consuming from Kafka. Topic/partition/offset: '@{Topic}/@{Partition}/@{Offset}'. Error: '@{Error}'.",
          string.Join(",", values.Select(a => a.Topic)),
          string.Join(",", values.Select(a => a.Partition)),
          string.Join(",", values.Select(consumer.Position)),
          error.Reason);
        throw new KafkaException(error);
      }
    }

    private readonly BrokerIngressKafkaDriverConfiguration _configuration;
    private string _brokerId;
    private IConsumer<Ignore, byte[]> _consumer;
    private Task<Task> _consumeTask;
    private bool _isInitialized;
    private ILogger<BrokerIngressKafkaDriver> _logger;
    private IMessageRouter _messageRouter;
    private IServiceProvider _serviceProvider;
  }
}
