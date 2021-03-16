#region Usings

using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  /// <summary>
  /// Parser of Kafka header value.
  /// </summary>
  /// <remarks>
  /// Kafka handles header values as byte arrays. Poezd handles broker headers as strings. This parser should translate bytes
  /// to string in accordance to way headers are encoded on producer side.
  /// </remarks>
  public interface IHeaderValueCodec
  {
    /// <summary>
    /// Converts Kafka header value from byte array to string.
    /// </summary>
    /// <remarks>
    /// Header values in Kafka can be <c>null</c> but it's not allowed in Poezd to get a header with <c>null</c> as its value.
    /// The implementation of this method should produce an empty string for <c>null</c> input value.
    /// </remarks>
    /// <param name="value">Byte array to parse.</param>
    /// <returns>
    /// String representation of header <paramref name="value" />.
    /// </returns>
    [Pure]
    [NotNull]
    string Decode([CanBeNull] byte[] value);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    [Pure]
    [NotNull]
    byte[] Encode([NotNull] string value);
  }
}
