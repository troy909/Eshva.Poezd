#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_broker_ingress
  {
    [Fact]
    public void when_constructed_it_should_initialize_properties()
    {
      var configuration = ConfigurationTests.CreateBrokerIngressConfiguration();
      configuration.EnterPipeFitterType = typeof(EmptyPipeFitter);
      configuration.ExitPipeFitterType = typeof(EmptyPipeFitter);

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());

      var sut = new BrokerIngress(
        new Mock<IMessageBroker>().Object,
        configuration,
        serviceProviderMock.Object);

      sut.Configuration.Should().BeSameAs(configuration);
      sut.Configuration.Driver.Should().BeSameAs(configuration.Driver);
      sut.EnterPipeFitter.Should().BeOfType<EmptyPipeFitter>();
      sut.ExitPipeFitter.Should().BeOfType<EmptyPipeFitter>();
      sut.Apis.Should().HaveCount(expected: 1);
    }

    [Fact]
    public void when_initialize_it_should_initialize_driver()
    {
      var configuration = ConfigurationTests.CreateBrokerIngressConfiguration();
      var driverMock = new Mock<IBrokerIngressDriver>();
      driverMock
        .Setup(
          driver => driver.Initialize(
            It.IsAny<IBrokerIngress>(),
            It.IsAny<IEnumerable<IIngressApi>>(),
            It.IsAny<IDiContainerAdapter>()))
        .Verifiable();
      configuration.Driver = driverMock.Object;

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());

      var sut = new BrokerIngress(
        Mock.Of<IMessageBroker>(),
        configuration,
        serviceProviderMock.Object);
      sut.Initialize();

      driverMock.Verify();
    }

    [Fact]
    public void when_initialized_twice_it_should_fail()
    {
      var configuration = ConfigurationTests.CreateBrokerIngressConfiguration();
      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());

      var ingress = new BrokerIngress(
        Mock.Of<IMessageBroker>(),
        configuration,
        serviceProviderMock.Object);

      Action sut = () => ingress.Initialize();

      sut.Should().NotThrow();
      sut.Should().ThrowExactly<PoezdOperationException>().Which.Message.Should().Contain(
        "already initialized",
        "it should not be possible to initialize ingress more than once");
    }

    [Fact]
    public void when_get_api_by_queue_name_it_should_return_api_handling_messages_from_this_queue_name()
    {
      var (configuration, serviceProvider) =
        ConfigurationTests.CreateBrokerIngressConfigurationWithTwoApisHandlingMessageFromDifferentQueues();
      var ingress = new BrokerIngress(
        Mock.Of<IMessageBroker>(),
        configuration,
        serviceProvider);
      ingress.Initialize();

      ingress.GetApiByQueueName("queue1").Id.Should().Be("api1", "api1 configured to handle messages from queue1");
      ingress.GetApiByQueueName("queue2").Id.Should().Be("api2", "api2 configured to handle messages from queue2");
    }

    [Fact]
    public void when_get_api_by_queue_name_and_no_any_api_handles_messages_from_specified_queue_it_should_fail()
    {
      var (configuration, serviceProvider) =
        ConfigurationTests.CreateBrokerIngressConfigurationWithTwoApisNotHandlingMessageFromAnyQueue();
      var ingress = new BrokerIngress(
        Mock.Of<IMessageBroker>(),
        configuration,
        serviceProvider);
      ingress.Initialize();

      const string queueName = "queue-name";
      Action sut = () => ingress.GetApiByQueueName(queueName);

      sut.Should().ThrowExactly<PoezdOperationException>().Which.Message.Should().Contain("doesn't belong to any API");
      sut.Should().ThrowExactly<PoezdOperationException>().Which.Message.Should().Contain(queueName);
    }

    [Fact]
    public void when_get_api_by_queue_name_and_a_few_apis_handle_messages_from_same_queue_it_should_fail()
    {
      var (configuration, serviceProvider) = ConfigurationTests.CreateBrokerIngressConfigurationWithTwoApisHandlingMessageFromAnyQueue();
      var ingress = new BrokerIngress(
        Mock.Of<IMessageBroker>(),
        configuration,
        serviceProvider);
      ingress.Initialize();

      const string queueName = "queue-name";
      Action sut = () => ingress.GetApiByQueueName(queueName);

      sut.Should().ThrowExactly<PoezdOperationException>().Which.Message.Should().Contain("belongs to a few APIs");
      sut.Should().ThrowExactly<PoezdOperationException>().Which.Message.Should().Contain(queueName);
    }

    [Fact]
    public void when_start_consume_messages_it_should_start_consume_messages_from_underlying_driver()
    {
      var driverMock = new Mock<IBrokerIngressDriver>();
      var driverStartConsumeMessagesCalled = false;
      driverMock.Setup(driver => driver.StartConsumeMessages(It.IsAny<IEnumerable<string>>(), CancellationToken.None))
        .Callback(() => driverStartConsumeMessagesCalled = true)
        .Returns(Task.CompletedTask);

      var configuration = ConfigurationTests.CreateBrokerIngressConfiguration()
        .With(ingressConfiguration => ingressConfiguration.Driver = driverMock.Object);
      var ingress = new BrokerIngress(
        Mock.Of<IMessageBroker>(),
        configuration,
        Mock.Of<IDiContainerAdapter>());
      ingress.Initialize();

      ingress.StartConsumeMessages(new[] {"queue name"});

      driverStartConsumeMessagesCalled.Should().BeTrue("broker ingress should start message consumption on driver");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_start_consume_messages_without_queue_names_it_should_fail()
    {
      var configuration = ConfigurationTests.CreateBrokerIngressConfiguration();
      var ingress = new BrokerIngress(
        Mock.Of<IMessageBroker>(),
        configuration,
        Mock.Of<IDiContainerAdapter>());
      ingress.Initialize();

      Action sut = () => ingress.StartConsumeMessages(queueNamePatterns: null);
      sut.Should().ThrowExactly<ArgumentNullException>().Which.ParamName.Should().Be("queueNamePatterns");
    }

    [Fact]
    public void when_start_consume_messages_on_uninitialized_ingress_it_should_fail()
    {
      var configuration = ConfigurationTests.CreateBrokerIngressConfiguration();
      var ingress = new BrokerIngress(
        Mock.Of<IMessageBroker>(),
        configuration,
        Mock.Of<IDiContainerAdapter>());

      Action sut = () => ingress.StartConsumeMessages(new[] {"queue name"});
      sut.Should().ThrowExactly<PoezdOperationException>().Which.Message.Should().Contain("not initialized");
    }

    // TODO: Write tests for other BrokerIngress members.
  }
}
