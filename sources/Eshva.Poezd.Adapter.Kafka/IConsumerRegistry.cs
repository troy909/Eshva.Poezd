#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  internal interface IConsumerRegistry : IDisposable
  {
    void Add<TKey, TValue>(IIngressApi api, IConsumer<TKey, TValue> consumer);

    IConsumer<TKey, TValue> Get<TKey, TValue>(IIngressApi api);
  }
}
