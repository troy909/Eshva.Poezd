#region Usings

using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_message_broker_configuration
  {
    [Fact]
    public void when_all_required_properties_set_it_should_be_validated()
    {
      var sut = ConfigurationTests.CreateMessageBrokerConfiguration();

      sut.Validate().Should().NotBeNull().And.Subject.Should().BeEmpty("there is no errors in the configuration");
    }

    [Fact]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      ConfigurationTests.CreateMessageBrokerConfigurationWithout(configuration => configuration.DriverConfiguration = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateMessageBrokerConfigurationWithout(configuration => configuration.DriverFactoryType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateMessageBrokerConfigurationWithout(configuration => configuration.Id = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateMessageBrokerConfigurationWithout(configuration => configuration.IngressEnterPipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateMessageBrokerConfigurationWithout(configuration => configuration.IngressExitPipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateMessageBrokerConfigurationWithout(configuration => configuration.QueueNameMatcherType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateMessageBrokerConfiguration(shouldAddPublicApi: false)
        .Validate().Should().HaveCount(expected: 1);
    }
  }
}
