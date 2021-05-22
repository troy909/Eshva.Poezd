#region Usings

using System;
using System.Linq;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using JetBrains.Annotations;
using Moq;
using RandomStringCreator;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_broker_ingress_configurator
  {
    [Fact]
    public void when_enter_pipe_fitter_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressConfiguration();
      var sut = new BrokerIngressConfigurator(configuration);
      sut.WithEnterPipeFitter<StabPipeFitter>().Should().BeSameAs(sut);
      configuration.EnterPipeFitterType.Should().Be<StabPipeFitter>();
    }

    [Fact]
    public void when_enter_pipe_fitter_set_more_than_once_it_should_fail()
    {
      var configuration = new BrokerIngressConfiguration();
      var configurator = new BrokerIngressConfigurator(configuration);
      Action sut = () => configurator.WithEnterPipeFitter<StabPipeFitter>();

      sut.Should().NotThrow();
      configuration.EnterPipeFitterType.Should().Be<StabPipeFitter>();
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    public void when_exit_pipe_fitter_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressConfiguration();
      var sut = new BrokerIngressConfigurator(configuration);
      sut.WithExitPipeFitter<StabPipeFitter>().Should().BeSameAs(sut);
      configuration.ExitPipeFitterType.Should().Be<StabPipeFitter>();
    }

    [Fact]
    public void when_exit_pipe_fitter_set_more_than_once_it_should_fail()
    {
      var configuration = new BrokerIngressConfiguration();
      var configurator = new BrokerIngressConfigurator(configuration);
      Action sut = () => configurator.WithExitPipeFitter<StabPipeFitter>();

      sut.Should().NotThrow();
      configuration.ExitPipeFitterType.Should().Be<StabPipeFitter>();
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    public void when_queue_name_matcher_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressConfiguration();
      var sut = new BrokerIngressConfigurator(configuration);
      sut.WithQueueNameMatcher<StabQueueNameMatcher>().Should().BeSameAs(sut);
      configuration.QueueNameMatcherType.Should().Be<StabQueueNameMatcher>();
    }

    [Fact]
    public void when_queue_name_matcher_set_more_than_once_it_should_fail()
    {
      var configuration = new BrokerIngressConfiguration();
      var configurator = new BrokerIngressConfigurator(configuration);
      Action sut = () => configurator.WithQueueNameMatcher<StabQueueNameMatcher>();

      sut.Should().NotThrow();
      configuration.QueueNameMatcherType.Should().Be<StabQueueNameMatcher>();
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    public void when_api_added_it_should_be_added_into_configuration()
    {
      var configuration = new BrokerIngressConfiguration();
      var sut = new BrokerIngressConfigurator(configuration);
      var expected = new StringCreator().Get(length: 10);
      sut.AddApi(api => api.WithId(expected)).Should().BeSameAs(sut);
      configuration.Apis.Should().HaveCount(expected: 1, "an API should be added")
        .And.Subject.Single().Id.Should().Be(expected, "it should be added API instance");
    }

    [Fact]
    public void when_constructed_without_configuration_object_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new BrokerIngressConfigurator(configuration: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_null_added_as_api_it_should_fail()
    {
      var configurator = new BrokerIngressConfigurator(new BrokerIngressConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.AddApi(configurator: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_set_driver_it_should_be_set_driver_and_driver_configuration()
    {
      var configuration = new BrokerIngressConfiguration();
      var sut = (IBrokerIngressDriverConfigurator) new BrokerIngressConfigurator(configuration);
      var expectedDriver = Mock.Of<IBrokerIngressDriver>();
      var expectedDriverConfiguration = Mock.Of<IMessageRouterConfigurationPart>();
      sut.SetDriver(expectedDriver, expectedDriverConfiguration);
      configuration.Driver.Should().BeSameAs(expectedDriver);
      configuration.DriverConfiguration.Should().BeSameAs(expectedDriverConfiguration);
    }

    [Fact]
    public void when_set_driver_more_than_once_it_should_fail()
    {
      var configuration = new BrokerIngressConfiguration();
      var configurator = (IBrokerIngressDriverConfigurator) new BrokerIngressConfigurator(configuration);
      var expectedDriver = Mock.Of<IBrokerIngressDriver>();
      var expectedDriverConfiguration = Mock.Of<IMessageRouterConfigurationPart>();
      Action sut = () => configurator.SetDriver(expectedDriver, expectedDriverConfiguration);

      sut.Should().NotThrow();
      configuration.Driver.Should().BeSameAs(expectedDriver);
      configuration.DriverConfiguration.Should().BeSameAs(expectedDriverConfiguration);
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    public void when_set_driver_with_null_as_driver_it_should_fail()
    {
      var configuration = new BrokerIngressConfiguration();
      var configurator = (IBrokerIngressDriverConfigurator) new BrokerIngressConfigurator(configuration);
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.SetDriver(driver: null, Mock.Of<IMessageRouterConfigurationPart>());
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("driver"), "driver is required");
    }

    [Fact]
    public void when_set_driver_with_null_as_driver_configuration_it_should_fail()
    {
      var configuration = new BrokerIngressConfiguration();
      var configurator = (IBrokerIngressDriverConfigurator) new BrokerIngressConfigurator(configuration);
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.SetDriver(Mock.Of<IBrokerIngressDriver>(), configuration: null);
      sut.Should().ThrowExactly<ArgumentNullException>().Where(
        exception => exception.ParamName.Equals("configuration"),
        "driver configuration is required");
    }

    private static void EnsureSecondCallOfConfigurationMethodFails(Action sut)
    {
      sut.Should().ThrowExactly<PoezdConfigurationException>().Which.Message.Should().Contain(
        "more than once",
        "configuration method should complain about it called twice with exception");
    }

    [UsedImplicitly]
    private class StabQueueNameMatcher : IQueueNameMatcher
    {
      public bool DoesMatch(string queueName, string queueNamePattern) => true;
    }

    [UsedImplicitly]
    private class StabPipeFitter : IPipeFitter
    {
      public void AppendStepsInto<TContext>(IPipeline<TContext> pipeline) where TContext : class { }
    }
  }
}
