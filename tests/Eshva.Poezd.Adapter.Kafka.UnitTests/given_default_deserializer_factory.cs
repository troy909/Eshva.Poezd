#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_default_deserializer_factory
  {
    [Fact]
    public void when_create_deserializer_for_known_data_type_it_should_return_expected_deserializer()
    {
      var sut = new DefaultDeserializerFactory();
      sut.Create<byte[]>().Should().Be(Deserializers.ByteArray);
      sut.Create<string>().Should().Be(Deserializers.Utf8);
      sut.Create<int>().Should().Be(Deserializers.Int32);
      sut.Create<long>().Should().Be(Deserializers.Int64);
      sut.Create<float>().Should().Be(Deserializers.Single);
      sut.Create<double>().Should().Be(Deserializers.Double);
      sut.Create<Ignore>().Should().Be(Deserializers.Ignore);
      sut.Create<Null>().Should().Be(Deserializers.Null);
    }

    [Fact]
    public void when_create_deserializer_for_unknown_data_type_it_should_return_null()
    {
      var sut = new DefaultDeserializerFactory();
      sut.Create<int[]>().Should().BeNull();
      sut.Create<byte>().Should().BeNull();
      sut.Create<char>().Should().BeNull();
      sut.Create<object>().Should().BeNull();
      sut.Create<DefaultDeserializerFactory>().Should().BeNull();
      sut.Create<IDeserializerFactory>().Should().BeNull();
      sut.Create<DateTimeOffset>().Should().BeNull();
    }
  }
}
