#region Usings

using System;
using Eshva.Poezd.Core.Configuration;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class ConfigurationTests
  {
    public static MessageBrokerConfiguration CreateMessageBrokerConfiguration(bool shouldAddPublicApi = true)
    {
      var someType = typeof(object);
      var sut = new MessageBrokerConfiguration
      {
        DriverConfiguration = new object(),
        DriverFactoryType = someType,
        Id = "id",
        IngressEnterPipeFitterType = someType,
        IngressExitPipeFitterType = someType,
        QueueNameMatcherType = someType
      };

      if (shouldAddPublicApi) sut.AddPublicApi(CreatePublicApiConfiguration());

      return sut;
    }

    public static MessageBrokerConfiguration CreateMessageBrokerConfigurationWithout(Action<MessageBrokerConfiguration> updater)
    {
      var configuration = CreateMessageBrokerConfiguration();
      updater(configuration);
      return configuration;
    }

    public static PublicApiConfiguration CreatePublicApiConfiguration()
    {
      var someType = typeof(object);
      var configuration = new PublicApiConfiguration
      {
        HandlerRegistryType = someType,
        Id = "id",
        IngressPipeFitterType = someType,
        MessageTypesRegistryType = someType,
        QueueNamePatternsProviderType = someType
      };

      return configuration;
    }

    public static PublicApiConfiguration CreatePublicApiConfigurationWithout(Action<PublicApiConfiguration> updater)
    {
      var configuration = CreatePublicApiConfiguration();
      updater(configuration);
      return configuration;
    }

    public static MessageRouterConfiguration CreateMessageRouterConfiguration()
    {
      var configuration = new MessageRouterConfiguration();
      configuration.AddBroker(ConfigurationTests.CreateMessageBrokerConfiguration());
      return configuration;
    }
  }
}
