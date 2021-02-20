#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using JetBrains.Annotations;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_message_broker_configurator
  {
    [Fact]
    public void when_id_set_it_should_be_set_in_configuration()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      const string expected = "id";
      sut.WithId(expected).Should().BeSameAs(sut);
      configuration.Id.Should().Be(expected);
    }

    [Fact]
    public void when_driver_set_it_should_be_set_in_configuration()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      var isConfiguratorCalled = false;
      sut.WithDriver<DriverFactory, DriverConfigurator, DriverConfiguration>(_ => isConfiguratorCalled = true).Should().BeSameAs(sut);
      configuration.DriverFactoryType.Should().Be(typeof(DriverFactory));
      configuration.DriverConfiguration.Should().NotBeNull().And.Subject.Should().BeOfType<DriverConfiguration>();
      isConfiguratorCalled.Should().BeTrue();
    }

    [Fact]
    public void when_ingress_enter_pipe_fitter_set_it_should_be_set_in_configurator()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      sut.WithIngressEnterPipeFitter<PipeFitter>().Should().BeSameAs(sut);
      configuration.IngressEnterPipeFitterType.Should().Be<PipeFitter>();
    }

    [Fact]
    public void when_ingress_exit_pipe_fitter_set_it_should_be_set_in_configurator()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      sut.WithIngressExitPipeFitter<PipeFitter>().Should().BeSameAs(sut);
      configuration.IngressExitPipeFitterType.Should().Be<PipeFitter>();
    }

    [Fact]
    public void when_queue_name_matcher_set_it_should_be_set_in_configuration()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      sut.WithQueueNameMatcher<QueueNameMatcher>().Should().BeSameAs(sut);
      configuration.QueueNameMatcherType.Should().Be<QueueNameMatcher>();
    }

    [Fact]
    public void when_public_api_added_it_should_be_added_into_configuration()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      sut.AddPublicApi(api => api.WithId("id")).Should().BeSameAs(sut);
      configuration.PublicApis.Should().HaveCount(expected: 1);
    }

    [Fact]
    public void when_constructed_without_configuration_object_it_should_fail()
    {
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      // ReSharper disable once ObjectCreationAsStatement
      Action sut = () => new MessageBrokerConfigurator(configuration: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_null_added_as_public_api_it_should_fail()
    {
      var configurator = new MessageBrokerConfigurator(new MessageBrokerConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.AddPublicApi(configurator: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_null_set_as_id_it_should_fail()
    {
      var configurator = new MessageBrokerConfigurator(new MessageBrokerConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.WithId(id: null);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_empty_string_set_as_id_it_should_fail()
    {
      var configurator = new MessageBrokerConfigurator(new MessageBrokerConfiguration());
      Action sut = () => configurator.WithId(string.Empty);
      sut.Should().Throw<ArgumentNullException>();
    }

    [Fact]
    public void when_whitespace_string_set_as_id_it_should_fail()
    {
      var configurator = new MessageBrokerConfigurator(new MessageBrokerConfiguration());
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.WithId(" \n\t");
      sut.Should().Throw<ArgumentNullException>();
    }

    [UsedImplicitly]
    private class QueueNameMatcher : IQueueNameMatcher
    {
      public bool DoesMatch(string queueName, string queueNamePattern) => true;
    }

    [UsedImplicitly]
    private class PipeFitter : IPipeFitter
    {
      public void Setup(IPipeline pipeline)
      {
        throw new NotImplementedException();
      }
    }

    private class DriverFactory : IMessageBrokerDriverFactory
    {
      public IMessageBrokerDriver Create() => new Driver();
    }

    [UsedImplicitly(ImplicitUseTargetFlags.Members)]
    private class Driver : IMessageBrokerDriver
    {
      public bool IsDisposed { get; private set; }

      public bool IsInitialized { get; private set; }

      public bool DoesConsumingMessages { get; private set; }

      public bool IsMessagePublished { get; private set; }

      public void Dispose()
      {
        IsDisposed = true;
      }

      public void Initialize(
        IMessageRouter messageRouter,
        string brokerId,
        object configuration)
      {
        IsInitialized = true;
      }

      public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
      {
        DoesConsumingMessages = true;
        return Task.CompletedTask;
      }

      public Task Publish(byte[] brokerPayload, IReadOnlyDictionary<string, string> brokerMetadata)
      {
        IsMessagePublished = true;
        return Task.CompletedTask;
      }
    }

    public class DriverConfigurator
    {
      // ReSharper disable once UnusedParameter.Local
      public DriverConfigurator(DriverConfiguration configuration) { }
    }

    public class DriverConfiguration { }
  }
}
