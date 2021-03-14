#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public interface IDeserializerFactory
  {
    [CanBeNull] public IDeserializer<TData> Create<TData>();
  }
}
