#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  public class DefaultSerializerFactory : ISerializerFactory
  {
    public ISerializer<TData> Create<TData>()
    {
      var type = typeof(TData);
      return type switch
      {
        { } when type == typeof(byte[]) => (ISerializer<TData>) Serializers.ByteArray,
        { } when type == typeof(string) => (ISerializer<TData>) Serializers.Utf8,
        { } when type == typeof(int) => (ISerializer<TData>) Serializers.Int32,
        { } when type == typeof(long) => (ISerializer<TData>) Serializers.Int64,
        { } when type == typeof(float) => (ISerializer<TData>) Serializers.Single,
        { } when type == typeof(double) => (ISerializer<TData>) Serializers.Double,
        { } when type == typeof(Null) => (ISerializer<TData>) Serializers.Null,
        _ => null
      };
    }
  }
}
