#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  internal class DefaultProducerRegistry : IProducerRegistry
  {
    public void Add<TKey, TValue>([NotNull] IEgressApi api, [NotNull] IProducer<TKey, TValue> producer)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));
      if (producer == null) throw new ArgumentNullException(nameof(producer));

      if (!_producers.TryAdd(api, producer))
        throw new PoezdConfigurationException($"An egress API with ID '{api.Id}' and its producer already registered.");
    }

    public IProducer<TKey, TValue> Get<TKey, TValue>([NotNull] IEgressApi api)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));

      if (!_producers.TryGetValue(api, out var producer))
        throw new ArgumentException($"There is no registered producers for API with ID '{api.Id}'.", nameof(api));

      return (IProducer<TKey, TValue>) producer;
    }

    public void Dispose()
    {
      foreach (var producer in _producers.Values)
      {
        ((IDisposable) producer).Dispose();
      }
    }

    private readonly Dictionary<IEgressApi, object> _producers = new();
  }
}
