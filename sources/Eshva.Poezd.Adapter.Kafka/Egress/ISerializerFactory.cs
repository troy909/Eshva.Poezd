#region Usings

using Confluent.Kafka;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.Egress
{
  /// <summary>
  /// The contract of a serializer factory for keys and values.
  /// </summary>
  public interface ISerializerFactory
  {
    /// <summary>
    /// Creates a serializer for <typeparamref name="TData" />.
    /// </summary>
    /// <typeparam name="TData">
    /// Type of data for which a serializer should be created.
    /// </typeparam>
    /// <returns>
    /// A serializer if <typeparamref name="TData" /> is known or <c>null</c> if it is unknown.
    /// </returns>
    [CanBeNull]
    public ISerializer<TData> Create<TData>();
  }
}
