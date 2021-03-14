#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using Confluent.Kafka;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  internal class DefaultConsumerRegistry : IConsumerRegistry
  {
    public void Add<TKey, TValue>(IIngressApi api, IConsumer<TKey, TValue> consumer)
    {
      if (!_consumers.TryAdd(api, consumer))
        throw new PoezdConfigurationException($"An ingress API with ID '{api.Id}' and its consumer already registered.");
    }

    public IConsumer<TKey, TValue> Get<TKey, TValue>(IIngressApi api)
    {
      var isConsumerFound = _consumers.TryGetValue(api, out var consumer);
      Debug.Assert(isConsumerFound, $"There is no registered consumer for API with ID '{api.Id}'.");
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
