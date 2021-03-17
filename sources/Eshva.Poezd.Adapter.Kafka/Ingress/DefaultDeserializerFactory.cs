#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  public class DefaultDeserializerFactory : IDeserializerFactory
  {
    public IDeserializer<TData> Create<TData>()
    {
      var type = typeof(TData);
      return type switch
      {
        { } when type == typeof(byte[]) => (IDeserializer<TData>) Deserializers.ByteArray,
        { } when type == typeof(string) => (IDeserializer<TData>) Deserializers.Utf8,
        { } when type == typeof(int) => (IDeserializer<TData>) Deserializers.Int32,
        { } when type == typeof(long) => (IDeserializer<TData>) Deserializers.Int64,
        { } when type == typeof(float) => (IDeserializer<TData>) Deserializers.Single,
        { } when type == typeof(double) => (IDeserializer<TData>) Deserializers.Double,
        { } when type == typeof(Ignore) => (IDeserializer<TData>) Deserializers.Ignore,
        { } when type == typeof(Null) => (IDeserializer<TData>) Deserializers.Null,
        _ => null
      };
    }
  }
}
