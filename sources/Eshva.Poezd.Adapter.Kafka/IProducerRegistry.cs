#region Usings

using System;
using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  internal interface IProducerRegistry : IDisposable
  {
    IProducer<TKey, TValue> Get<TKey, TValue>(ProducerConfig config);
  }
}
