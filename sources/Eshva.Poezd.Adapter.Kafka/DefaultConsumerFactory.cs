#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class DefaultConsumerFactory : IConsumerFactory
  {
    public DefaultConsumerFactory(ILogger<DefaultConsumerFactory> logger)
    {
      _logger = logger;
    }

    // TODO: Add integration tests for DefaultConsumerFactory and DefaultProducerFactory.
    public IConsumer<TKey, TValue> Create<TKey, TValue>(
      ConsumerConfig consumerConfig,
      IConsumerConfigurator configurator,
      IDeserializerFactory deserializerFactory)
    {
      try
      {
        _logger.LogInformation("Creating Kafka consumer with configuration @{ConsumerConfig}", consumerConfig);
        var consumer = configurator.Configure(
            new ConsumerBuilder<TKey, TValue>(consumerConfig),
            deserializerFactory.Create<TKey>(),
            deserializerFactory.Create<TValue>())
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

    private readonly ILogger<DefaultConsumerFactory> _logger;
  }
}
