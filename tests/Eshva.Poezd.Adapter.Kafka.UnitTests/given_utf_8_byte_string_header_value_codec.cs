#region Usings

using System.Text;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_utf_8_byte_string_header_value_codec
  {
    [Fact]
    public void when_decode_it_should_produce_original_string()
    {
      const string expectedValue = "Поезд едет 🚃";
      var sut = new Utf8ByteStringHeaderValueCodec();

      var value = sut.Decode(Encoding.UTF8.GetBytes(expectedValue));

      value.Should().Be(expectedValue, "value was encoded as UTF-8");
    }

    [Fact]
    public void when_decode_zero_length_byte_array_it_should_return_empty_string()
    {
      var sut = new Utf8ByteStringHeaderValueCodec();

      var value = sut.Decode(new byte[0]);

      value.Should().BeEmpty($"it is the contract of {nameof(IHeaderValueCodec)}");
    }

    [Fact]
    public void when_decode_null_value_it_should_return_empty_string()
    {
      var sut = new Utf8ByteStringHeaderValueCodec();

      var value = sut.Decode(value: null);

      value.Should().BeEmpty($"it is the contract of {nameof(IHeaderValueCodec)}");
    }

    [Fact]
    public void when_encode_it_should_produce_original_byte_array()
    {
      const string expected = "Поезд едет 🚃";

      var sut = new Utf8ByteStringHeaderValueCodec();

      var value = sut.Encode(expected);
      value.Should().BeEquivalentTo(Encoding.UTF8.GetBytes(expected));
    }

    [Fact]
    public void when_encode_null_it_should_produce_zero_length_array()
    {
      var sut = new Utf8ByteStringHeaderValueCodec();

      var value = sut.Encode(value: null);
      value.Should().BeEquivalentTo(new byte[0]);
    }

    [Fact]
    public void when_encode_empty_string_it_should_produce_zero_length_array()
    {
      var sut = new Utf8ByteStringHeaderValueCodec();

      var value = sut.Encode(string.Empty);
      value.Should().BeEquivalentTo(new byte[0]);
    }
  }
}
