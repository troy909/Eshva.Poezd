#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests.Tools
{
  [SuppressMessage("ReSharper", "UnassignedGetOnlyAutoProperty")]
  public class TestProducer<TKey, TValue> : IProducer<TKey, TValue>, ITestableProducer
  {
    public TestProducer(
      ProducerConfig config,
      Dictionary<string, object> publishedMessages,
      Exception exceptionToThrowOnPublishing = default)
    {
      _publishedMessages = publishedMessages;
      _exceptionToThrowOnPublishing = exceptionToThrowOnPublishing;
      Config = config;
    }

    public ProducerConfig Config { get; }

    /// <inheritdoc />
    public Handle Handle { get; }

    /// <inheritdoc />
    public string Name { get; }

    public bool IsDisposed { get; private set; }

    /// <inheritdoc />
    public void Dispose()
    {
      IsDisposed = true;
    }

    /// <inheritdoc />
    public int AddBrokers(string brokers) => 0;

    /// <inheritdoc />
    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(
      string topic,
      Message<TKey, TValue> message,
      CancellationToken cancellationToken = new CancellationToken()) =>
      PublishInternal(
        topic,
        message,
        cancellationToken);

    /// <inheritdoc />
    public Task<DeliveryResult<TKey, TValue>> ProduceAsync(
      TopicPartition topicPartition,
      Message<TKey, TValue> message,
      CancellationToken cancellationToken = new CancellationToken())
    {
      var topic = topicPartition.Topic;
      return PublishInternal(
        topic,
        message,
        cancellationToken);
    }

    /// <inheritdoc />
    public void Produce(
      string topic,
      Message<TKey, TValue> message,
      Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
    {
      PublishInternal(
        topic,
        message,
        CancellationToken.None);
    }

    /// <inheritdoc />
    public void Produce(
      TopicPartition topicPartition,
      Message<TKey, TValue> message,
      Action<DeliveryReport<TKey, TValue>> deliveryHandler = null)
    {
      var topic = topicPartition.Topic;
      PublishInternal(
        topic,
        message,
        CancellationToken.None);
    }

    /// <inheritdoc />
    public int Poll(TimeSpan timeout) => 0;

    /// <inheritdoc />
    public int Flush(TimeSpan timeout) => 0;

    /// <inheritdoc />
    public void Flush(CancellationToken cancellationToken = new CancellationToken()) { }

    /// <inheritdoc />
    public void InitTransactions(TimeSpan timeout) { }

    /// <inheritdoc />
    public void BeginTransaction() { }

    /// <inheritdoc />
    public void CommitTransaction(TimeSpan timeout) { }

    /// <inheritdoc />
    public void CommitTransaction() { }

    /// <inheritdoc />
    public void AbortTransaction(TimeSpan timeout) { }

    /// <inheritdoc />
    public void AbortTransaction() { }

    /// <inheritdoc />
    public void SendOffsetsToTransaction(
      IEnumerable<TopicPartitionOffset> offsets,
      IConsumerGroupMetadata groupMetadata,
      TimeSpan timeout) { }

    private Task<DeliveryResult<TKey, TValue>> PublishInternal(
      string topic,
      Message<TKey, TValue> message,
      CancellationToken cancellationToken)
    {
      if (_exceptionToThrowOnPublishing != null) throw _exceptionToThrowOnPublishing;

      _publishedMessages.Add(topic, message);
      return cancellationToken != default
        ? Task.FromCanceled<DeliveryResult<TKey, TValue>>(cancellationToken)
        : Task.FromResult<DeliveryResult<TKey, TValue>>(new DeliveryReport<TKey, TValue>());
    }

    private readonly Exception _exceptionToThrowOnPublishing;

    private readonly Dictionary<string, object> _publishedMessages;
  }
}
