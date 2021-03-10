#region Usings

using System;
using System.Collections.Concurrent;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  internal class CachingProducerRegistry : IProducerRegistry
  {
    public CachingProducerRegistry([NotNull] IProducerFactory producerFactory)
    {
      _producerFactory = producerFactory ?? throw new ArgumentNullException(nameof(producerFactory));
    }

    public IProducer<TKey, TValue> Get<TKey, TValue>(ProducerConfig config)
    {
      try
      {
        var producerKey = HashCode.Combine(
          typeof(TKey),
          typeof(TValue),
          config);
        return (IProducer<TKey, TValue>) _producers.GetOrAdd(producerKey, _producerFactory.Create<TKey, TValue>(config));
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException(
          "During Kafka producer creating an occurred error. Inspect the internal exception for more details",
          exception);
      }
    }

    public void Dispose()
    {
      foreach (var producer in _producers.Values)
      {
        ((IDisposable) producer).Dispose();
      }
    }

    private readonly IProducerFactory _producerFactory;
    private readonly ConcurrentDictionary<int, object> _producers = new();
  }
}
