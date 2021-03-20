#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// The default serializer factory for keys and values.
  /// </summary>
  /// <remarks>
  /// Use this class as a base for your serializer factories if your key or value types differ from supported by the
  /// Confluent .NET Kafka API or use it itself if you use only supported. See <see cref="Serializers" /> the find the
  /// supported data types.
  /// </remarks>
  public class DefaultSerializerFactory : ISerializerFactory
  {
    /// <inheritdoc />
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
