#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  public interface IApiConsumer<TKey, TValue> : IDisposable
  {
    Task Start(
      Func<ConsumeResult<TKey, TValue>, Task> onMessageReceived,
      CancellationToken cancellationToken);

    void Stop();
  }
}
