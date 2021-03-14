#region Usings

using System.Text;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  /// <summary>
  /// Header value parser for UTF-8 encoded byte arrays.
  /// </summary>
  public class Utf8ByteStringHeaderValueCodec : IHeaderValueCodec
  {
    /// <inheritdoc />
    public string Decode(byte[] value) => value != null ? Encoding.UTF8.GetString(value) : string.Empty;

    /// <inheritdoc />
    public byte[] Encode(string value) => Encoding.UTF8.GetBytes(value);
  }
}
