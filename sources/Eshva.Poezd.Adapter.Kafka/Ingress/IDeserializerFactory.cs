#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  public interface IDeserializerFactory
  {
    [CanBeNull] public IDeserializer<TData> Create<TData>();
  }
}
