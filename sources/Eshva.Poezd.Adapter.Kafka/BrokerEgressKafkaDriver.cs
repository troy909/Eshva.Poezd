#region Usings

using System;
using System.Collections.Generic;
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
    public BrokerEgressKafkaDriver([NotNull] BrokerEgressKafkaDriverConfiguration configuration)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <inheritdoc />
    public void Initialize(string brokerId, ILogger<IBrokerEgressDriver> logger)
    {
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));
      _brokerId = brokerId;

      // _configuration = configuration as KafkaDriverConfiguration ?? throw new ArgumentException(
      // $"The value of {nameof(configuration)} parameter should be of type {typeof(KafkaDriverConfiguration).FullName}.");
      // if (configuration == null) throw new ArgumentNullException(nameof(configuration));

      if (_isInitialized)
        throw new PoezdOperationException($"Kafka driver for broker with ID '{_brokerId}' is already initialized.");

      _producer = CreateProducer();

      _isInitialized = true;
    }

    public Task Publish(
      byte[] key,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames)
    {
      if (!_isInitialized) throw new PoezdOperationException("Kafka driver should be initialized before it can publish messages.");
      // TODO: Add message publishing.

      return Task.CompletedTask;
    }

    /// <inheritdoc />
    public void Dispose()
    {
      _producer?.Dispose();
    }

    private IProducer<Null, byte[]> CreateProducer()
    {
      try
      {
        _logger.LogInformation("Creating Kafka producer with configuration @{ProducerConfig}", _configuration.ProducerConfig);
        var producer = new ProducerBuilder<Null, byte[]>(_configuration.ProducerConfig).Build();
        _logger.LogInformation("A Kafka producer created.");
        return producer;
      }
      catch (Exception exception)
      {
        const string errorMessage = "During Kafka producer creating an error occurred.";
        _logger.LogError(exception, errorMessage);
        throw new PoezdOperationException(errorMessage, exception);
      }
    }

    private readonly BrokerEgressKafkaDriverConfiguration _configuration;
    private string _brokerId;
    private bool _isInitialized;
    private ILogger<IBrokerEgressDriver> _logger;
    private IProducer<Null, byte[]> _producer;
  }
}
