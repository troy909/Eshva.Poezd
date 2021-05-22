#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using Moq;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public static class ConfigurationTests
  {
    public static MessageBrokerConfiguration CreateMessageBrokerConfiguration()
    {
      var sut = new MessageBrokerConfiguration
      {
        Id = "id",
        Ingress = CreateBrokerIngressConfiguration(),
        Egress = CreateBrokerEgressConfiguration()
      };

      return sut;
    }

    public static MessageBrokerConfiguration With(
      this MessageBrokerConfiguration configuration,
      Action<MessageBrokerConfiguration> updater)
    {
      updater(configuration);
      return configuration;
    }

    public static BrokerIngressConfiguration CreateBrokerIngressConfiguration(bool shouldAddApis = true)
    {
      var someType = typeof(object);
      var configuration = new BrokerIngressConfiguration
      {
        Driver = Mock.Of<IBrokerIngressDriver>(),
        DriverConfiguration = CreateDriverConfiguration(),
        EnterPipeFitterType = someType,
        ExitPipeFitterType = someType,
        QueueNameMatcherType = someType
      };

      if (shouldAddApis) configuration.AddApi(CreateIngressApiConfiguration());

      return configuration;
    }

    public static BrokerIngressConfiguration With(
      this BrokerIngressConfiguration configuration,
      Action<BrokerIngressConfiguration> updater)
    {
      updater(configuration);
      return configuration;
    }

    public static (BrokerIngressConfiguration, IDiContainerAdapter)
      CreateBrokerIngressConfigurationWithTwoApisHandlingMessageFromDifferentQueues()
    {
      var configuration = CreateBrokerIngressConfiguration(shouldAddApis: false)
        .With(ingressConfiguration => ingressConfiguration.QueueNameMatcherType = typeof(RegexQueueNameMatcher));

      configuration.AddApi(
        CreateIngressApiConfiguration().With(
          api =>
          {
            api.Id = "api1";
            api.QueueNamePatternsProviderType = typeof(MatchingQueue1QueueNamePatternsProvider);
          }));
      configuration.AddApi(
        CreateIngressApiConfiguration().With(
          api =>
          {
            api.Id = "api2";
            api.QueueNamePatternsProviderType = typeof(MatchingQueue2QueueNamePatternsProvider);
          }));

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(RegexQueueNameMatcher)))
        .Returns(() => new RegexQueueNameMatcher());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(TestQueueNamePatternsProvider)))
        .Returns(() => new TestQueueNamePatternsProvider());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(MatchingQueue1QueueNamePatternsProvider)))
        .Returns(() => new MatchingQueue1QueueNamePatternsProvider());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(MatchingQueue2QueueNamePatternsProvider)))
        .Returns(() => new MatchingQueue2QueueNamePatternsProvider());

      return (configuration, serviceProviderMock.Object);
    }

    public static (BrokerIngressConfiguration, IDiContainerAdapter)
      CreateBrokerIngressConfigurationWithTwoApisHandlingMessageFromAnyQueue()
    {
      var configuration = CreateBrokerIngressConfiguration(shouldAddApis: false)
        .With(ingressConfiguration => ingressConfiguration.QueueNameMatcherType = typeof(MatchingEverythingQueueNameMatcher));

      configuration.AddApi(
        CreateIngressApiConfiguration().With(
          api =>
          {
            api.Id = "api1";
            api.QueueNamePatternsProviderType = typeof(TestQueueNamePatternsProvider);
          }));
      configuration.AddApi(
        CreateIngressApiConfiguration().With(
          api =>
          {
            api.Id = "api2";
            api.QueueNamePatternsProviderType = typeof(TestQueueNamePatternsProvider);
          }));

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(MatchingEverythingQueueNameMatcher)))
        .Returns(() => new MatchingEverythingQueueNameMatcher());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(TestQueueNamePatternsProvider)))
        .Returns(() => new TestQueueNamePatternsProvider());

      return (configuration, serviceProviderMock.Object);
    }

    public static (BrokerIngressConfiguration, IDiContainerAdapter)
      CreateBrokerIngressConfigurationWithTwoApisNotHandlingMessageFromAnyQueue()
    {
      var configuration = CreateBrokerIngressConfiguration(shouldAddApis: false)
        .With(ingressConfiguration => ingressConfiguration.QueueNameMatcherType = typeof(MatchingNothingQueueNameMatcher));

      configuration.AddApi(
        CreateIngressApiConfiguration().With(
          api =>
          {
            api.Id = "api1";
            api.QueueNamePatternsProviderType = typeof(TestQueueNamePatternsProvider);
          }));
      configuration.AddApi(
        CreateIngressApiConfiguration().With(
          api =>
          {
            api.Id = "api2";
            api.QueueNamePatternsProviderType = typeof(TestQueueNamePatternsProvider);
          }));

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(MatchingNothingQueueNameMatcher)))
        .Returns(() => new MatchingNothingQueueNameMatcher());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(TestQueueNamePatternsProvider)))
        .Returns(() => new TestQueueNamePatternsProvider());

      return (configuration, serviceProviderMock.Object);
    }

    public static IngressApiConfiguration CreateIngressApiConfiguration() => new IngressApiConfiguration
    {
      HandlerRegistryType = typeof(object),
      Id = "id",
      PipeFitterType = typeof(object),
      MessageTypesRegistryType = typeof(object),
      QueueNamePatternsProviderType = typeof(object),
      MessageKeyType = typeof(object),
      MessagePayloadType = typeof(object)
    };

    public static IngressApiConfiguration With(
      this IngressApiConfiguration configuration,
      Action<IngressApiConfiguration> updater)
    {
      updater(configuration);
      return configuration;
    }

    public static EgressApiConfiguration CreateEgressApiConfiguration() => new EgressApiConfiguration
    {
      Id = "id",
      MessageTypesRegistryType = typeof(object),
      PipeFitterType = typeof(object),
      MessageKeyType = typeof(object),
      MessagePayloadType = typeof(object)
    };

    public static EgressApiConfiguration With(
      this EgressApiConfiguration configuration,
      Action<EgressApiConfiguration> updater)
    {
      updater(configuration);
      return configuration;
    }

    public static MessageRouterConfiguration CreateMessageRouterConfiguration()
    {
      var configuration = new MessageRouterConfiguration();
      configuration.AddBroker(CreateMessageBrokerConfiguration());
      return configuration;
    }

    public static BrokerEgressConfiguration CreateBrokerEgressConfiguration(bool shouldAddApis = true)
    {
      var configuration = new BrokerEgressConfiguration
      {
        Driver = new TestBrokerEgressDriver(new TestDriverState()),
        DriverConfiguration = new TestBrokerEgressDriverConfiguration(),
        EnterPipeFitterType = typeof(object),
        ExitPipeFitterType = typeof(object)
      };
      if (shouldAddApis) configuration.AddApi(CreateEgressApiConfiguration());

      return configuration;
    }

    public static BrokerEgressConfiguration With(
      this BrokerEgressConfiguration configuration,
      Action<BrokerEgressConfiguration> updater)
    {
      updater(configuration);
      return configuration;
    }

    public static IMessageRouterConfigurationPart CreateDriverConfiguration() => Mock.Of<IMessageRouterConfigurationPart>();

    private class MatchingQueue1QueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      public IEnumerable<string> GetQueueNamePatterns() => new[] {"queue1"};
    }

    private class MatchingQueue2QueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      public IEnumerable<string> GetQueueNamePatterns() => new[] {"queue2"};
    }

    private class TestQueueNamePatternsProvider : IQueueNamePatternsProvider
    {
      public IEnumerable<string> GetQueueNamePatterns() => new[] {"queue name"};
    }
  }
}
