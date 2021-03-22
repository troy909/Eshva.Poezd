#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// The default producer registry.
  /// </summary>
  internal class DefaultProducerRegistry : IProducerRegistry
  {
    /// <inheritdoc />
    public void Add<TKey, TValue>(IEgressApi api, IProducer<TKey, TValue> producer)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));
      if (producer == null) throw new ArgumentNullException(nameof(producer));

      if (!_producers.TryAdd(api, producer))
        throw new PoezdConfigurationException($"An egress API with ID '{api.Id}' and its producer already registered.");
    }

    /// <inheritdoc />
    public IProducer<TKey, TValue> Get<TKey, TValue>(IEgressApi api)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));

      if (!_producers.TryGetValue(api, out var producer))
        throw new ArgumentException($"There is no registered producers for API with ID '{api.Id}'.", nameof(api));

      return (IProducer<TKey, TValue>) producer;
    }

    /// <inheritdoc />
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
