#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_default_serializer_factory
  {
    [Fact]
    public void when_create_serializer_for_known_data_type_it_should_return_expected_serializer()
    {
      var sut = new DefaultSerializerFactory();
      sut.Create<byte[]>().Should().Be(Serializers.ByteArray);
      sut.Create<string>().Should().Be(Serializers.Utf8);
      sut.Create<int>().Should().Be(Serializers.Int32);
      sut.Create<long>().Should().Be(Serializers.Int64);
      sut.Create<float>().Should().Be(Serializers.Single);
      sut.Create<double>().Should().Be(Serializers.Double);
      sut.Create<Null>().Should().Be(Serializers.Null);
    }

    [Fact]
    public void when_create_serializer_for_unknown_data_type_it_should_return_null()
    {
      var sut = new DefaultSerializerFactory();
      sut.Create<int[]>().Should().BeNull();
      sut.Create<byte>().Should().BeNull();
      sut.Create<char>().Should().BeNull();
      sut.Create<object>().Should().BeNull();
      sut.Create<DefaultSerializerFactory>().Should().BeNull();
      sut.Create<ISerializerFactory>().Should().BeNull();
      sut.Create<DateTimeOffset>().Should().BeNull();
    }
  }
}
