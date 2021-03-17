#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  internal interface IProducerRegistry : IDisposable
  {
    void Add<TKey, TValue>(IEgressApi api, IProducer<TKey, TValue> producer);

    IProducer<TKey, TValue> Get<TKey, TValue>(IEgressApi api);
  }
}
