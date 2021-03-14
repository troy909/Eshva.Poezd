#region Usings

using System;
using System.Collections.Generic;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  internal class DefaultConsumerRegistry : IConsumerRegistry
  {
    public void Add<TKey, TValue>([NotNull] IIngressApi api, [NotNull] IConsumer<TKey, TValue> consumer)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));
      if (consumer == null) throw new ArgumentNullException(nameof(consumer));

      if (!_consumers.TryAdd(api, consumer))
        throw new PoezdConfigurationException($"An ingress API with ID '{api.Id}' and its consumer already registered.");
    }

    public IConsumer<TKey, TValue> Get<TKey, TValue>([NotNull] IIngressApi api)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));

      if (!_consumers.TryGetValue(api, out var consumer))
        throw new ArgumentException($"There is no registered consumer for API with ID '{api.Id}'.", nameof(api));

      return (IConsumer<TKey, TValue>) consumer;
    }

    public void Dispose()
    {
      foreach (var consumer in _consumers.Values)
      {
        ((IDisposable) consumer).Dispose();
      }
    }

    private readonly Dictionary<IIngressApi, object> _consumers = new();
  }
}
