#region Usings

using System.Text;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_utf8_byte_string_header_parser
  {
    [Fact]
    public void when_utf8_string_encoded_as_bytes_parsed_it_should_produce_original_string()
    {
      const string expectedValue = "Поезд едет 🚃";
      var parser = new Utf8ByteStringHeaderValueCodec();

      var value = parser.Decode(Encoding.UTF8.GetBytes(expectedValue));

      value.Should().Be(expectedValue, "value was encoded as UTF-8");
    }

    [Fact]
    public void when_zero_length_byte_array_supplied_it_should_return_empty_string()
    {
      var parser = new Utf8ByteStringHeaderValueCodec();

      var value = parser.Decode(new byte[0]);

      value.Should().BeEmpty($"it is the contract of {nameof(IHeaderValueCodec)}");
    }

    [Fact]
    public void when_null_header_value_supplied_it_should_return_empty_string()
    {
      var parser = new Utf8ByteStringHeaderValueCodec();

      var value = parser.Decode(value: null);

      value.Should().BeEmpty($"it is the contract of {nameof(IHeaderValueCodec)}");
    }
  }
}
