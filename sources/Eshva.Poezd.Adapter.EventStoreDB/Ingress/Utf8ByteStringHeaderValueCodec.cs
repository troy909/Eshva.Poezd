#region Usings

using System.Text;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  /// <summary>
  /// UTF-8 header value codec.
  /// </summary>
  public class Utf8ByteStringHeaderValueCodec : IHeaderValueCodec
  {
    /// <inheritdoc />
    public string Decode(byte[] value) => value != null ? Encoding.UTF8.GetString(value) : string.Empty;

    /// <inheritdoc />
    public byte[] Encode(string value) => value != null ? Encoding.UTF8.GetBytes(value) : new byte[0];
  }
}
