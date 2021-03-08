#region Usings

using System;
using System.Linq;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using JetBrains.Annotations;
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
    public void when_exit_pipe_fitter_set_it_should_be_set_in_configuration()
    {
      var configuration = new BrokerIngressConfiguration();
      var sut = new BrokerIngressConfigurator(configuration);
      sut.WithExitPipeFitter<StabPipeFitter>().Should().BeSameAs(sut);
      configuration.ExitPipeFitterType.Should().Be<StabPipeFitter>();
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
