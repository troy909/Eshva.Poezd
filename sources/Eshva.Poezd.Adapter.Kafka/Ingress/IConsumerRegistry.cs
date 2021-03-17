#region Usings

using System;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  internal interface IConsumerRegistry : IDisposable
  {
    void Add<TKey, TValue>(IIngressApi api, IApiConsumer<TKey, TValue> consumer);

    IApiConsumer<TKey, TValue> Get<TKey, TValue>(IIngressApi api);
  }
}
