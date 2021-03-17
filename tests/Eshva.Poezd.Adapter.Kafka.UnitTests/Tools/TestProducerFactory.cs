#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests.Tools
{
  public class TestProducerFactory : IProducerFactory
  {
    public TestProducerFactory(
      Dictionary<string, object> publishedMessages,
      Exception exceptionToThrowOnPublishing = default)
    {
      _publishedMessages = publishedMessages;
      _exceptionToThrowOnPublishing = exceptionToThrowOnPublishing;
    }

    public IProducer<TKey, TValue> Create<TKey, TValue>(
      ProducerConfig config,
      IProducerConfigurator configurator,
      ISerializerFactory serializerFactory) =>
      new TestProducer<TKey, TValue>(
        config,
        _publishedMessages,
        _exceptionToThrowOnPublishing);

    private readonly Exception _exceptionToThrowOnPublishing;
    private readonly Dictionary<string, object> _publishedMessages;
  }
}
