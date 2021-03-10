#region Usings

using Confluent.Kafka;
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
      var brokerEgressConfigurator = new BrokerEgressConfigurator(configuration);
      brokerEgressConfigurator.WithKafkaDriver(
        driver => driver
          .WithProducerConfig(new ProducerConfig())
          .WithDefaultProducerFactory());

      configuration.Driver.Should().BeOfType<BrokerEgressKafkaDriver>("what it can be else?");
    }
  }
}
