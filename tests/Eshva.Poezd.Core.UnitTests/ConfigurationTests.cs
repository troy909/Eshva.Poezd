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

    public static MessageBrokerConfiguration CreateMessageBrokerConfigurationWithout(Action<MessageBrokerConfiguration> updater)
    {
      var configuration = CreateMessageBrokerConfiguration();
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

    public static BrokerIngressConfiguration CreateBrokerIngressConfigurationWithout(Action<BrokerIngressConfiguration> updater)
    {
      var configuration = CreateBrokerIngressConfiguration();
      updater(configuration);
      return configuration;
    }

    public static (BrokerIngressConfiguration, IDiContainerAdapter)
      CreateBrokerIngressConfigurationWithTwoApisHandlingMessageFromDifferentQueues()
    {
      var configuration = new BrokerIngressConfiguration
      {
        Driver = Mock.Of<IBrokerIngressDriver>(),
        DriverConfiguration = CreateDriverConfiguration(),
        EnterPipeFitterType = typeof(object),
        ExitPipeFitterType = typeof(object),
        QueueNameMatcherType = typeof(RegexQueueNameMatcher)
      };

      var apiConfiguration1 = CreateNamedIngressApiConfiguration<MatchingQueue1QueueNamePatternsProvider>("api1");
      configuration.AddApi(apiConfiguration1);
      var apiConfiguration2 = CreateNamedIngressApiConfiguration<MatchingQueue2QueueNamePatternsProvider>("api2");
      configuration.AddApi(apiConfiguration2);

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

      IngressApiConfiguration CreateNamedIngressApiConfiguration<TQueueNamePatternsProvider>(string id)
        where TQueueNamePatternsProvider : IQueueNamePatternsProvider
      {
        return new IngressApiConfiguration
        {
          HandlerRegistryType = typeof(object),
          Id = id,
          PipeFitterType = typeof(object),
          MessageTypesRegistryType = typeof(object),
          QueueNamePatternsProviderType = typeof(TQueueNamePatternsProvider),
          MessageKeyType = typeof(object),
          MessagePayloadType = typeof(object)
        };
      }
    }

    public static (BrokerIngressConfiguration, IDiContainerAdapter)
      CreateBrokerIngressConfigurationWithTwoApisHandlingMessageFromAnyQueue()
    {
      var configuration = new BrokerIngressConfiguration
      {
        Driver = Mock.Of<IBrokerIngressDriver>(),
        DriverConfiguration = CreateDriverConfiguration(),
        EnterPipeFitterType = typeof(object),
        ExitPipeFitterType = typeof(object),
        QueueNameMatcherType = typeof(MatchingEverythingQueueNameMatcher)
      };

      var apiConfiguration1 = CreateNamedIngressApiConfiguration("api1");
      configuration.AddApi(apiConfiguration1);
      var apiConfiguration2 = CreateNamedIngressApiConfiguration("api2");
      configuration.AddApi(apiConfiguration2);

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(MatchingEverythingQueueNameMatcher)))
        .Returns(() => new MatchingEverythingQueueNameMatcher());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(TestQueueNamePatternsProvider)))
        .Returns(() => new TestQueueNamePatternsProvider());

      return (configuration, serviceProviderMock.Object);

      IngressApiConfiguration CreateNamedIngressApiConfiguration(string id)
      {
        return new IngressApiConfiguration
        {
          HandlerRegistryType = typeof(object),
          Id = id,
          PipeFitterType = typeof(object),
          MessageTypesRegistryType = typeof(object),
          QueueNamePatternsProviderType = typeof(TestQueueNamePatternsProvider),
          MessageKeyType = typeof(object),
          MessagePayloadType = typeof(object)
        };
      }
    }

    public static (BrokerIngressConfiguration, IDiContainerAdapter)
      CreateBrokerIngressConfigurationWithTwoApisNotHandlingMessageFromAnyQueue()
    {
      var configuration = new BrokerIngressConfiguration
      {
        Driver = Mock.Of<IBrokerIngressDriver>(),
        DriverConfiguration = CreateDriverConfiguration(),
        EnterPipeFitterType = typeof(object),
        ExitPipeFitterType = typeof(object),
        QueueNameMatcherType = typeof(MatchingNothingQueueNameMatcher)
      };

      var apiConfiguration1 = CreateNamedIngressApiConfiguration("api1");
      configuration.AddApi(apiConfiguration1);
      var apiConfiguration2 = CreateNamedIngressApiConfiguration("api2");
      configuration.AddApi(apiConfiguration2);

      var serviceProviderMock = new Mock<IDiContainerAdapter>();
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(EmptyPipeFitter))).Returns(() => new EmptyPipeFitter());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(MatchingNothingQueueNameMatcher)))
        .Returns(() => new MatchingNothingQueueNameMatcher());
      serviceProviderMock.Setup(adapter => adapter.GetService(typeof(TestQueueNamePatternsProvider)))
        .Returns(() => new TestQueueNamePatternsProvider());

      return (configuration, serviceProviderMock.Object);

      IngressApiConfiguration CreateNamedIngressApiConfiguration(string id)
      {
        return new IngressApiConfiguration
        {
          HandlerRegistryType = typeof(object),
          Id = id,
          PipeFitterType = typeof(object),
          MessageTypesRegistryType = typeof(object),
          QueueNamePatternsProviderType = typeof(TestQueueNamePatternsProvider),
          MessageKeyType = typeof(object),
          MessagePayloadType = typeof(object)
        };
      }
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

    public static IngressApiConfiguration CreateIngressApiConfigurationWithout(Action<IngressApiConfiguration> updater)
    {
      var configuration = CreateIngressApiConfiguration();
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

    public static EgressApiConfiguration CreateEgressApiConfigurationWithout(Action<EgressApiConfiguration> updater)
    {
      var configuration = CreateEgressApiConfiguration();
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

    public static BrokerEgressConfiguration CreateBrokerEgressConfigurationWithout(Action<BrokerEgressConfiguration> updater)
    {
      var configuration = CreateBrokerEgressConfiguration();
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
