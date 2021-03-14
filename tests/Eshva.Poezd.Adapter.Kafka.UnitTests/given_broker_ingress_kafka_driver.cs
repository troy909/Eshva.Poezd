#region Usings

using System;
using FluentAssertions;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_broker_ingress_kafka_driver
  {
    [Fact]
    public void when_constructed_with_invalid_arguments_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new BrokerIngressKafkaDriver(configuration: null, Mock.Of<IConsumerRegistry>());
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("configuration", "null is not valid configuration");
    }
  }
}
