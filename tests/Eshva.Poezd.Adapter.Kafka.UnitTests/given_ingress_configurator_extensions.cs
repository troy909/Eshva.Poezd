#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_ingress_configurator_extensions
  {
    [Fact]
    public void when_set_kafka_driver_it_should_set_driver_on_broker_ingress()
    {
      var configuration = MakeConfiguration();
      var configurator = new BrokerIngressConfigurator(configuration);
      configurator.WithKafkaDriver(
        driverConfigurator => driverConfigurator
          .WithHeaderValueCodec<IHeaderValueCodec>()
          .WithConsumerConfig(new ConsumerConfig())
          .WithConsumerConfigurator<IConsumerConfigurator>()
          .WithConsumerFactory<IApiConsumerFactory>()
          .WithDeserializerFactory<IDeserializerFactory>());

      configuration.Driver.Should().BeOfType<BrokerIngressKafkaDriver>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_set_kafka_driver_without_configurator_it_should_fail()
    {
      var configuration = MakeConfiguration();
      var configurator = new BrokerIngressConfigurator(configuration);
      Action sut = () => configurator.WithKafkaDriver(configurator: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    private static BrokerIngressConfiguration MakeConfiguration()
    {
      var configuration = new BrokerIngressConfiguration();
      var configurator = new BrokerIngressConfigurator(configuration);
      configurator
        .WithEnterPipeFitter<IPipeFitter>()
        .WithExitPipeFitter<IPipeFitter>()
        .WithQueueNameMatcher<RegexQueueNameMatcher>()
        .AddApi(
          apiConfigurator => apiConfigurator
            .WithHandlerRegistry<IHandlerRegistry>()
            .WithId("id")
            .WithMessageKey<int>()
            .WithMessagePayload<string>()
            .WithMessageTypesRegistry<IIngressApiMessageTypesRegistry>()
            .WithPipeFitter<IPipeFitter>()
            .WithQueueNamePatternsProvider<IQueueNamePatternsProvider>());

      return configuration;
    }
  }
}
