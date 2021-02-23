#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;

#endregion

namespace Venture.Common.TestingTools.Kafka
{
  public class KafkaTestContext<TValue> : IAsyncDisposable
  {
    public KafkaTestContext(string bootstrapServers, CancellationToken cancellationToken = default)
    {
      if (string.IsNullOrWhiteSpace(bootstrapServers))
        throw new ArgumentException("Value cannot be null or whitespace.", nameof(bootstrapServers));

      _bootstrapServers = bootstrapServers;
      _cancellationToken = cancellationToken;
    }

    public Task CreateTopics(params string[] topicNames)
    {
      if (topicNames == null) throw new ArgumentNullException(nameof(topicNames));
      if (topicNames.Length == 0) throw new ArgumentException("No topics to create specified.", nameof(topicNames));
      if (IsTopicCreatingBlocked) throw new InvalidOperationException("You can not create topics if already consumed messages.");

      EnsureAdminClient();
      _createdTopics.AddRange(topicNames);
      return _adminClient.CreateTopicsAsync(topicNames.Select(topicName => new TopicSpecification {Name = topicName, NumPartitions = 1}));
    }

    public async Task<DeliveryResult<Null, TValue>> Produce(
      string topicName,
      TValue value,
      IDictionary<string, byte[]> headers = default)
    {
      if (value == null) throw new ArgumentNullException(nameof(value));
      if (string.IsNullOrWhiteSpace(topicName)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(topicName));

      EnsureProducer();

      var headersToSend = new Headers();
      if (headers != null)
      {
        foreach (var (headerKey, headerValue) in headers)
        {
          headersToSend.Add(headerKey, headerValue);
        }
      }

      var result = await _producer.ProduceAsync(
        topicName,
        new Message<Null, TValue> {Value = value, Headers = headersToSend},
        _cancellationToken);
      _producer.Flush(_cancellationToken);
      return result;
    }

    public ConsumeResult<Ignore, TValue> Consume(string topicName)
    {
      if (string.IsNullOrWhiteSpace(topicName)) throw new ArgumentNullException(nameof(topicName));

      EnsureConsumer();
      return _consumer.Consume(_cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
      if (_adminClient != null)
      {
        try
        {
          if (_createdTopics.Any()) await _adminClient.DeleteTopicsAsync(_createdTopics);
        }
        catch (DeleteTopicsException)
        {
          // Ignore topic deleting errors.
        }

        _adminClient?.Dispose();
      }

      _producer?.Dispose();
      _consumer?.Dispose();
    }

    private void EnsureAdminClient()
    {
      if (_adminClient != null) return;

      _adminClient = new AdminClientBuilder(new AdminClientConfig {BootstrapServers = _bootstrapServers}).Build();
    }

    private void EnsureProducer()
    {
      if (_producer != null) return;

      _producer = new ProducerBuilder<Null, TValue>(new ProducerConfig {BootstrapServers = _bootstrapServers}).Build();
    }

    private void EnsureConsumer()
    {
      if (_consumer != null) return;

      _consumer = new ConsumerBuilder<Ignore, TValue>(
        new ConsumerConfig
        {
          BootstrapServers = _bootstrapServers,
          GroupId = Guid.NewGuid().ToString("N"),
          AutoOffsetReset = AutoOffsetReset.Earliest,
          AllowAutoCreateTopics = true
        }).Build();

      _consumer.Subscribe(_createdTopics);
    }

    private bool IsTopicCreatingBlocked => _consumer != null;

    private readonly string _bootstrapServers;
    private readonly CancellationToken _cancellationToken;
    private readonly List<string> _createdTopics = new List<string>();
    private IAdminClient _adminClient;
    private IConsumer<Ignore, TValue> _consumer;
    private IProducer<Null, TValue> _producer;
  }
}
