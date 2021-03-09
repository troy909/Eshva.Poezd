#region Usings

using System.Text;

#endregion

namespace Eshva.Poezd.Adapter.Kafka
{
  /// <summary>
  /// Header value parser for UTF-8 encoded byte arrays.
  /// </summary>
  public class Utf8ByteStringHeaderValueParser : IHeaderValueParser
  {
    /// <inheritdoc />
    public string Parser(byte[] value) => value != null ? Encoding.UTF8.GetString(value) : string.Empty;
  }
}
