#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Routing;

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
        Ingress = BrokerIngressConfiguration.Empty,
        Egress = BrokerEgressConfiguration.Empty
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
        Driver = new StabBrokerIngressDriver(),
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
      QueueNamePatternsProviderType = typeof(object)
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
      PipeFitterType = typeof(object)
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

    private class StabBrokerIngressDriver : IBrokerIngressDriver
    {
      public void Dispose()
      {
        throw new NotImplementedException();
      }

      public void Initialize(
        IMessageRouter messageRouter,
        string brokerId,
        IServiceProvider serviceProvider)
      {
        throw new NotImplementedException();
      }

      public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default) =>
        throw new NotImplementedException();
    }
  }
}
