#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class BrokerEgressKafkaDriver : IBrokerEgressDriver
  {
    internal BrokerEgressKafkaDriver(
      [NotNull] BrokerEgressKafkaDriverConfiguration configuration,
      [NotNull] IProducerRegistry producerRegistry)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _producerRegistry = producerRegistry ?? throw new ArgumentNullException(nameof(producerRegistry));
    }

    /// <inheritdoc />
    public void Initialize(
      string brokerId,
      ILogger<IBrokerEgressDriver> logger,
      IClock clock)
    {
      if (_isInitialized) throw new PoezdOperationException($"Kafka driver for broker with ID {_brokerId} is already initialized.");
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _clock = clock ?? throw new ArgumentNullException(nameof(clock));
      _brokerId = brokerId;
      _isInitialized = true;
    }

    /// <inheritdoc />
    public Task Publish(
      object key,
      object payload,
      IEgressApi api,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames,
      CancellationToken cancellationToken)
    {
      if (!_isInitialized) throw new PoezdOperationException("Kafka driver should be initialized before it can publish messages.");
      if (key == null) throw new ArgumentNullException(nameof(key));
      if (payload == null) throw new ArgumentNullException(nameof(payload));
      if (api == null) throw new ArgumentNullException(nameof(api));
      if (metadata == null) throw new ArgumentNullException(nameof(metadata));
      if (queueNames == null) throw new ArgumentNullException(nameof(queueNames));

      var publish = PublishMethod!.MakeGenericMethod(key.GetType(), payload.GetType());
      return ((Task) publish.Invoke(this, new[] {key, payload, api, metadata, queueNames, cancellationToken}))!;
    }

    /// <inheritdoc />
    public void Dispose()
    {
      _producerRegistry.Dispose();
    }

    private async Task Publish<TKey, TValue>(
      TKey key,
      TValue payload,
      IEgressApi api,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames,
      CancellationToken cancellationToken)
    {
      var producer = _producerRegistry.Get<TKey, TValue>(api);
      var headers = MakeHeaders(metadata);
      var timestamp = new Timestamp(_clock.GetNowUtc());
      foreach (var queueName in queueNames)
      {
        var message = new Message<TKey, TValue> {Key = key, Value = payload, Headers = headers, Timestamp = timestamp};
        try
        {
          _logger.LogDebug(
            "Publishing a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
            key,
            queueName,
            _brokerId);
          await producer.ProduceAsync(
            queueName,
            message,
            cancellationToken);
          _logger.LogDebug(
            "Successfully published a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
            key,
            queueName,
            _brokerId);
        }
        catch (Exception exception)
        {
          _logger.LogDebug(
            exception,
            "Failed to publish a message with key {Key} to topic {Topic} of broker with ID {BrokerId}.",
            key,
            queueName,
            _brokerId);
          throw;
        }
      }
    }

    private static Headers MakeHeaders(IReadOnlyDictionary<string, string> metadata)
    {
      var headers = new Headers();
      foreach (var (key, value) in metadata)
      {
        headers.Add(key, Encoding.UTF8.GetBytes(value));
      }

      return headers;
    }

    private readonly BrokerEgressKafkaDriverConfiguration _configuration;
    private readonly IProducerRegistry _producerRegistry;
    private string _brokerId;
    private IClock _clock;
    private bool _isInitialized;
    private ILogger<IBrokerEgressDriver> _logger;

    private static readonly MethodInfo PublishMethod =
      typeof(BrokerEgressKafkaDriver).GetMethod(nameof(Publish), BindingFlags.Instance | BindingFlags.NonPublic);
  }
}
