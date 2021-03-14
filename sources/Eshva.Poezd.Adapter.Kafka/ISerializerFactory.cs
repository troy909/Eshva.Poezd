#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public interface ISerializerFactory
  {
    [CanBeNull] public ISerializer<TData> Create<TData>();
  }
}
