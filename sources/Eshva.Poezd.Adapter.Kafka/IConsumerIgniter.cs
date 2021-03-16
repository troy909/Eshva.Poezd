#region Usings

using System;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public interface IConsumerIgniter<TKey, TValue>
  {
    Task Start(
      IConsumer<TKey, TValue> consumer,
      Func<ConsumeResult<TKey, TValue>, Task> onMessageReceived,
      CancellationToken cancellationToken);
  }
}
