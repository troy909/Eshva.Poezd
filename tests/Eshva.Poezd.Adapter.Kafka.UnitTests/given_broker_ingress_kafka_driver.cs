#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_broker_ingress_kafka_driver
  {
    [Fact]
    public void when_constructed_with_valid_arguments_it_should_success()
    {
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new BrokerIngressKafkaDriver(MakeDriverConfiguration(), Mock.Of<IConsumerRegistry>());
      sut.Should().NotThrow("all parameters provided");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_constructed_with_invalid_arguments_it_should_fail()
    {
      BrokerIngressKafkaDriverConfiguration configuration = null;
      var consumerRegistry = Mock.Of<IConsumerRegistry>();
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new BrokerIngressKafkaDriver(configuration, consumerRegistry);
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should()
        .Be("configuration", "null is not valid configuration");
      configuration = new BrokerIngressKafkaDriverConfiguration();
      consumerRegistry = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should()
        .Be("consumerRegistry", "null is not valid consumer registry");
      consumerRegistry = Mock.Of<IConsumerRegistry>();

      configuration = new BrokerIngressKafkaDriverConfiguration
      {
        ConsumerConfiguratorType = typeof(object),
        ConsumerFactoryType = typeof(object),
        DeserializerFactoryType = typeof(object)
      };

      configuration.ConsumerConfiguratorType = null;
      sut.Should().ThrowExactly<ArgumentException>().Which.ParamName.Should()
        .Be("configuration", "null is not valid consumer configuration type");
      configuration.ConsumerConfiguratorType = typeof(object);
      configuration.ConsumerFactoryType = null;
      sut.Should().ThrowExactly<ArgumentException>().Which.ParamName.Should()
        .Be("configuration", "null is not valid consumer factory type");
      configuration.ConsumerFactoryType = typeof(object);
      configuration.DeserializerFactoryType = null;
      sut.Should().ThrowExactly<ArgumentException>().Which.ParamName.Should()
        .Be("configuration", "null is not valid deserializer factory type");
      configuration.DeserializerFactoryType = typeof(object);
      configuration.HeaderValueCodecType = null;
      sut.Should().ThrowExactly<ArgumentException>().Which.ParamName.Should()
        .Be("configuration", "null is not valid deserializer factory type");
    }

    [Fact]
    public void when_initialize_it_should_create_consumers_for_each_api()
    {
      var consumers = 0;
      var consumerRegistryMock = new Mock<IConsumerRegistry>();
      consumerRegistryMock.Setup(
          registry => registry
            .Add(It.IsAny<IIngressApi>(), It.IsAny<IApiConsumer<It.IsAnyType, It.IsAnyType>>()))
        .Callback(() => consumers++);
      var configuration = MakeDriverConfiguration();

      var sut = new BrokerIngressKafkaDriver(configuration, consumerRegistryMock.Object);

      sut.Initialize(
        "broker-1",
        Mock.Of<IMessageRouter>(),
        new[] {MakeApi<int, byte[]>(), MakeApi<int, string>()},
        MakeServiceProviderMock().Object);

      consumers.Should().Be(expected: 2, "driver was initialized with 2 APIs");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_initialized_with_invalid_arguments_it_should_fail()
    {
      var driver = new BrokerIngressKafkaDriver(MakeDriverConfiguration(), Mock.Of<IConsumerRegistry>());
      var brokerId = "broker-1";
      var messageRouter = Mock.Of<IMessageRouter>();
      var apis = new[] {MakeApi<int, string>()};
      var serviceProvider = MakeServiceProviderMock().Object;

      Action sut = () => driver.Initialize(
        brokerId,
        messageRouter,
        apis,
        serviceProvider);

      brokerId = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("brokerId"));
      brokerId = string.Empty;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("brokerId"));
      brokerId = WhitespaceString;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("brokerId"));
      brokerId = "broker-1";

      messageRouter = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("messageRouter"));
      messageRouter = Mock.Of<IMessageRouter>();

      apis = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("apis"));
      apis = new[] {MakeApi<int, string>()};

      serviceProvider = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("serviceProvider"));
    }

    [Fact]
    public void when_initialized_with_service_provider_missing_required_services_it_should_fail()
    {
      var configuration = MakeDriverConfiguration();
      var consumerRegistry = Mock.Of<IConsumerRegistry>();
      var driver = new BrokerIngressKafkaDriver(configuration, consumerRegistry);

      var serviceProviderMock = MakeServiceProviderMock();
      Action sut = () => driver.Initialize(
        "broker-1",
        Mock.Of<IMessageRouter>(),
        new[] {MakeApi<int, string>()},
        serviceProviderMock.Object);

      serviceProviderMock.Setup(provider => provider.GetService(typeof(TestConsumerConfigurator))).Returns(valueFunction: null);
      sut.Should().ThrowExactly<ArgumentException>().Where(exception => exception.ParamName.Equals("serviceProvider"));
      serviceProviderMock.Setup(provider => provider.GetService(typeof(TestConsumerConfigurator))).Returns(new TestConsumerConfigurator());

      serviceProviderMock.Setup(provider => provider.GetService(typeof(TestConsumerFactory))).Returns(valueFunction: null);
      sut.Should().ThrowExactly<ArgumentException>().Where(exception => exception.ParamName.Equals("serviceProvider"));
      serviceProviderMock.Setup(provider => provider.GetService(typeof(TestConsumerFactory))).Returns(new TestConsumerFactory());

      serviceProviderMock.Setup(provider => provider.GetService(typeof(TestDeserializerFactory))).Returns(valueFunction: null);
      sut.Should().ThrowExactly<ArgumentException>().Where(exception => exception.ParamName.Equals("serviceProvider"));
      serviceProviderMock.Setup(provider => provider.GetService(typeof(TestDeserializerFactory))).Returns(new TestDeserializerFactory());

      serviceProviderMock.Setup(provider => provider.GetService(typeof(TestHeaderValueCodec))).Returns(valueFunction: null);
      sut.Should().ThrowExactly<ArgumentException>().Where(exception => exception.ParamName.Equals("serviceProvider"));
      serviceProviderMock.Setup(provider => provider.GetService(typeof(TestHeaderValueCodec))).Returns(new TestHeaderValueCodec());

      serviceProviderMock.Setup(provider => provider.GetService(typeof(ILoggerFactory))).Returns(valueFunction: null);
      sut.Should().ThrowExactly<ArgumentException>().Where(exception => exception.ParamName.Equals("serviceProvider"));
      serviceProviderMock.Setup(provider => provider.GetService(typeof(ILoggerFactory))).Returns(Mock.Of<ILoggerFactory>());
    }

    [Fact]
    public void when_initializing_twice_it_should_fail()
    {
      var driver = new BrokerIngressKafkaDriver(MakeDriverConfiguration(), Mock.Of<IConsumerRegistry>());

      Action sut = () => driver.Initialize(
        "broker-1",
        Mock.Of<IMessageRouter>(),
        new[] {MakeApi<int, byte[]>(), MakeApi<int, string>()},
        MakeServiceProviderMock().Object);

      sut.Should().NotThrow();
      sut.Should().ThrowExactly<PoezdOperationException>().Where(exception => exception.Message.Contains("already initialized"));
    }

    [Fact]
    public void when_start_consume_messages_but_not_initialized_it_should_fail()
    {
      var driver = new BrokerIngressKafkaDriver(MakeDriverConfiguration(), Mock.Of<IConsumerRegistry>());
      Action sut = () => driver.StartConsumeMessages(new[] {"topic-1", "topic-2"}, CancellationToken.None);
      sut.Should().ThrowExactly<PoezdOperationException>().Where(exception => exception.Message.Contains("initialized"));
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_start_consume_messages_with_invalid_arguments_it_should_fail()
    {
      var driver = new BrokerIngressKafkaDriver(MakeDriverConfiguration(), Mock.Of<IConsumerRegistry>());
      driver.Initialize(
        "broker-1",
        Mock.Of<IMessageRouter>(),
        new[] {MakeApi<int, byte[]>(), MakeApi<int, string>()},
        MakeServiceProviderMock().Object);
      Action sut = () => driver.StartConsumeMessages(queueNamePatterns: null, CancellationToken.None);
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("queueNamePatterns"));
    }

    [Fact]
    public void when_dispose_it_should_dispose_consumer_registry()
    {
      var consumerRegistryMock = new Mock<IConsumerRegistry>();
      consumerRegistryMock.Setup(registry => registry.Dispose()).Verifiable();
      var driver = new BrokerIngressKafkaDriver(MakeDriverConfiguration(), consumerRegistryMock.Object);

      driver.Dispose();

      consumerRegistryMock.Verify(registry => registry.Dispose(), Times.Once());
    }

    private static BrokerIngressKafkaDriverConfiguration MakeDriverConfiguration()
    {
      var configuration = new BrokerIngressKafkaDriverConfiguration
      {
        ConsumerConfig = new ConsumerConfig(),
        ConsumerConfiguratorType = typeof(TestConsumerConfigurator),
        ConsumerFactoryType = typeof(TestConsumerFactory),
        DeserializerFactoryType = typeof(TestDeserializerFactory),
        HeaderValueCodecType = typeof(TestHeaderValueCodec)
      };
      return configuration;
    }

    private static Mock<IServiceProvider> MakeServiceProviderMock()
    {
      var mock = new Mock<IServiceProvider>();
      mock.Setup(provider => provider.GetService(typeof(TestDeserializerFactory))).Returns(new TestDeserializerFactory());
      mock.Setup(provider => provider.GetService(typeof(TestConsumerFactory))).Returns(new TestConsumerFactory());
      mock.Setup(provider => provider.GetService(typeof(TestConsumerConfigurator))).Returns(new TestConsumerConfigurator());
      mock.Setup(provider => provider.GetService(typeof(TestHeaderValueCodec))).Returns(new TestHeaderValueCodec());
      mock.Setup(provider => provider.GetService(typeof(ILoggerFactory))).Returns(Mock.Of<ILoggerFactory>());
      return mock;
    }

    private static IIngressApi MakeApi<TKey, TValue>()
    {
      var mock = new Mock<IIngressApi>();
      mock.SetupGet(api => api.MessageKeyType).Returns(typeof(TKey));
      mock.SetupGet(api => api.MessagePayloadType).Returns(typeof(TValue));
      return mock.Object;
    }

    private const string WhitespaceString = " \t\n\r";

    private class TestHeaderValueCodec : IHeaderValueCodec
    {
      public string Decode(byte[] value) => string.Empty;

      public byte[] Encode(string value) => new byte[0];
    }

    private class TestDeserializerFactory : IDeserializerFactory
    {
      public IDeserializer<TData> Create<TData>() => null;
    }

    private class TestConsumerConfigurator : IConsumerConfigurator
    {
      public ConsumerBuilder<TKey, TValue> Configure<TKey, TValue>(
        ConsumerBuilder<TKey, TValue> builder,
        IDeserializer<TKey> keyDeserializer,
        IDeserializer<TValue> valueDeserializer) =>
        builder;
    }

    private class TestConsumerFactory : IConsumerFactory
    {
      public IConsumer<TKey, TValue> Create<TKey, TValue>(
        ConsumerConfig config,
        IConsumerConfigurator configurator,
        IDeserializerFactory deserializerFactory) =>
        Mock.Of<IConsumer<TKey, TValue>>();
    }
  }
}
