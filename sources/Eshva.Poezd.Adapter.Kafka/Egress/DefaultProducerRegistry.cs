#region Usings

using System;
using System.Collections.Generic;
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
    public void Add(IEgressApi api, IApiProducer producer)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));
      if (producer == null) throw new ArgumentNullException(nameof(producer));

      if (!_producers.TryAdd(api, producer))
        throw new PoezdConfigurationException($"An egress API with ID '{api.Id}' and its producer already registered.");
    }

    /// <inheritdoc />
    public IApiProducer Get(IEgressApi api)
    {
      if (api == null) throw new ArgumentNullException(nameof(api));

      if (!_producers.TryGetValue(api, out var producer))
        throw new ArgumentException($"There is no registered producers for API with ID '{api.Id}'.", nameof(api));

      return (IApiProducer) producer;
    }

    /// <inheritdoc />
    public void Dispose()
    {
      foreach (var producer in _producers.Values)
      {
        ((IDisposable) producer).Dispose();
      }
    }

    private readonly Dictionary<IEgressApi, object> _producers = new Dictionary<IEgressApi, object>();
  }
}
