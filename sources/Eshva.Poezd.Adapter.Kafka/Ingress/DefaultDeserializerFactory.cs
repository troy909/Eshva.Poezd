#region Usings

using Confluent.Kafka;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// The default deserializer factory for keys and values.
  /// </summary>
  /// <remarks>
  /// Use this class as a base for your deserializer factories if your key or value types differ from supported by the
  /// Confluent .NET Kafka API or use it itself if you use only supported. See <see cref="Deserializers" /> the find the
  /// supported data types.
  /// </remarks>
  public class DefaultDeserializerFactory : IDeserializerFactory
  {
    /// <inheritdoc />
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
