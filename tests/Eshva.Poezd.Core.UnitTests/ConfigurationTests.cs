#region Usings

using System;
using Eshva.Poezd.Core.Configuration;
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

    private static IMessageRouterConfigurationPart CreateDriverConfiguration() => Mock.Of<IMessageRouterConfigurationPart>();
  }
}
