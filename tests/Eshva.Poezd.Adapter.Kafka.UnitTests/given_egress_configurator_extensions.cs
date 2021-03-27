#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_egress_configurator_extensions
  {
    [Fact]
    public void when_broker_egress_configured_with_kafka_driver_it_should_configure_in_expected_way()
    {
      var configuration = new BrokerEgressConfiguration();
      var configurator = new BrokerEgressConfigurator(configuration);
      configurator.WithKafkaDriver(
        driver => driver
          .WithProducerConfig(new ProducerConfig())
          .WithDefaultProducerFactory()
          .WithHeaderValueCodec<IHeaderValueCodec>()
          .WithProducerFactory<IApiProducerFactory>()
          .WithSerializerFactory<ISerializerFactory>());

      configuration.Driver.Should().BeOfType<BrokerEgressKafkaDriver>("what it can be else?");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_set_kafka_driver_without_configurator_it_should_fail()
    {
      var configuration = new BrokerEgressConfiguration();
      var configurator = new BrokerEgressConfigurator(configuration);
      Action sut = () => configurator.WithKafkaDriver(configurator: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }
  }
}
