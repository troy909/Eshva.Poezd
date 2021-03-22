#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Ingress
{
  /// <summary>
  /// The contract of deserializer factory for keys and values.
  /// </summary>
  [PublicAPI]
  public interface IDeserializerFactory
  {
    /// <summary>
    /// Creates a deserializer for <typeparamref name="TData" />.
    /// </summary>
    /// <typeparam name="TData">
    /// Type of data for which a deserializer should be created.
    /// </typeparam>
    /// <returns>
    /// A deserializer if <typeparamref name="TData" /> is known or <c>null</c> if it is unknown.
    /// </returns>
    [CanBeNull]
    public IDeserializer<TData> Create<TData>();
  }
}
