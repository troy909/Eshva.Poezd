#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// The default consumer registry.
  /// </summary>
  internal class DefaultConsumerRegistry : IConsumerRegistry
  {
    /// <inheritdoc />
    public void Add<TKey, TValue>([NotNull] IIngressApi api, [NotNull] IApiConsumer<TKey, TValue> consumer)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));
      if (consumer == null) throw new ArgumentNullException(nameof(consumer));

      if (!_consumers.TryAdd(api, consumer))
        throw new PoezdConfigurationException($"An ingress API with ID '{api.Id}' and its consumer already registered.");
    }

    /// <inheritdoc />
    public IApiConsumer<TKey, TValue> Get<TKey, TValue>([NotNull] IIngressApi api)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));

      if (!_consumers.TryGetValue(api, out var consumer))
        throw new ArgumentException($"There is no registered consumer for API with ID '{api.Id}'.", nameof(api));

      return (IApiConsumer<TKey, TValue>) consumer;
    }

    /// <inheritdoc />
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
