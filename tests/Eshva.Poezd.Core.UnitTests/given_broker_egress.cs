#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_broker_egress
  {
    [Fact]
    public void when_constructed_it_should_initialize_properties()
    {
      var configuration = ConfigurationTests.CreateBrokerEgressConfiguration();
      configuration.EnterPipeFitterType = typeof(EmptyPipeFitter);
      configuration.ExitPipeFitterType = typeof(EmptyPipeFitter);

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());

      var sut = new BrokerEgress(
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
      var configuration = ConfigurationTests.CreateBrokerEgressConfiguration();
      var driverMock = new Mock<IBrokerEgressDriver>();
      driverMock
        .Setup(
          driver => driver.Initialize(
            It.IsAny<IEnumerable<IEgressApi>>(),
            It.IsAny<IDiContainerAdapter>()))
        .Verifiable();
      configuration.Driver = driverMock.Object;

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());

      var sut = new BrokerEgress(
        configuration,
        serviceProviderMock.Object);
      sut.Initialize();

      driverMock.Verify();
    }

    [Fact]
    public void when_publish_it_should_publish_to_driver()
    {
      var configuration = ConfigurationTests.CreateBrokerEgressConfiguration();
      var driverMock = new Mock<IBrokerEgressDriver>();
      driverMock
        .Setup(driver => driver.Publish(It.IsAny<MessagePublishingContext>(), It.IsAny<CancellationToken>()))
        .Verifiable();
      configuration.Driver = driverMock.Object;

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());

      var sut = new BrokerEgress(
        configuration,
        serviceProviderMock.Object);
      sut.Publish(new MessagePublishingContext(), CancellationToken.None);

      driverMock.Verify();
    }

    [Fact]
    public void when_dispose_it_should_dispose_driver()
    {
      var configuration = ConfigurationTests.CreateBrokerEgressConfiguration();
      var driverMock = new Mock<IBrokerEgressDriver>();
      driverMock.Setup(driver => driver.Dispose()).Verifiable();
      configuration.Driver = driverMock.Object;

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());

      var sut = new BrokerEgress(
        configuration,
        serviceProviderMock.Object);
      sut.Dispose();

      driverMock.Verify();
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_constructed_with_invalid_arguments_it_should_fail()
    {
      var configuration = ConfigurationTests.CreateBrokerEgressConfiguration();
      var serviceProvider = Mock.Of<IDiContainerAdapter>();

      Action sut = () => new BrokerEgress(
        configuration,
        serviceProvider);

      configuration = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("configuration"));
      configuration = new BrokerEgressConfiguration();

      serviceProvider = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("serviceProvider"));
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_constructed_with_invalid_configuration_it_should_fail()
    {
      var egressConfiguration = ConfigurationTests.CreateBrokerEgressConfiguration().With(configuration => configuration.Driver = null);
      var serviceProvider = Mock.Of<IDiContainerAdapter>();

      Action sut = () => new BrokerEgress(
        egressConfiguration,
        serviceProvider);

      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("configuration.Driver"));
    }

    [Fact]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_constructed_with_invalid_pipe_fitters_in_configuration_it_should_fail()
    {
      var configuration = ConfigurationTests.CreateBrokerEgressConfiguration();
      configuration.EnterPipeFitterType = typeof(IPipeFitter);
      configuration.ExitPipeFitterType = typeof(IPipeFitter);
      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(It.IsAny<Type>())).Throws<InvalidOperationException>();

      Action sut = () => new BrokerEgress(
        configuration,
        serviceProviderMock.Object);

      configuration.EnterPipeFitterType = typeof(object);
      sut.Should().ThrowExactly<PoezdConfigurationException>().Where(exception => exception.Message.Contains(typeof(object).FullName));
      configuration.ExitPipeFitterType = typeof(IPipeFitter);

      configuration.ExitPipeFitterType = typeof(object);
      sut.Should().ThrowExactly<PoezdConfigurationException>().Where(exception => exception.Message.Contains(typeof(object).FullName));
    }
  }
}
