#region Usings

using System;
using System.Reflection;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_broker_ingress_configuration
  {
    [Fact]
    public void when_all_required_properties_set_it_should_be_validated()
    {
      var sut = ConfigurationTests.CreateBrokerIngressConfiguration();

      sut.Validate().Should().NotBeNull().And.Subject.Should().BeEmpty("there is no errors in the configuration");
    }

    [Fact]
    public void when_validating_it_should_validate_expected_number_of_properties()
    {
      const int expectedNumberOfValidatingProperties = 5;
      const int numberOfNotValidatingPublicProperties = 1;

      var validatingProperties = typeof(BrokerIngressConfiguration).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      validatingProperties.Should().HaveCount(
        expectedNumberOfValidatingProperties + numberOfNotValidatingPublicProperties,
        $"seams like you've added new properties to {nameof(BrokerIngressConfiguration)}. " +
        $"Update its ValidateItself method to test them or update {nameof(expectedNumberOfValidatingProperties)} const value.");
    }

    [Fact]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      ConfigurationTests.CreateBrokerIngressConfigurationWithout(configuration => configuration.Driver = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerIngressConfigurationWithout(configuration => configuration.DriverConfiguration = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerIngressConfigurationWithout(configuration => configuration.EnterPipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerIngressConfigurationWithout(configuration => configuration.ExitPipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerIngressConfigurationWithout(configuration => configuration.QueueNameMatcherType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerIngressConfiguration(shouldAddApis: false)
        .Validate().Should().HaveCount(expected: 1);
    }

    [Fact]
    public void when_same_api_added_second_time_it_should_fail()
    {
      var configuration = new BrokerIngressConfiguration();
      var api = new IngressApiConfiguration();
      Action sut = () => { configuration.AddApi(api); };
      sut.Should().NotThrow();
      sut.Should().ThrowExactly<PoezdConfigurationException>();
    }

    [Fact]
    public void when_api_with_same_id_added_it_should_fail()
    {
      var configuration = new BrokerIngressConfiguration();

      const string sameId = "same id";
      var api1 = new IngressApiConfiguration {Id = sameId};
      Action sut1 = () => { configuration.AddApi(api1); };
      sut1.Should().NotThrow();

      var api2 = new IngressApiConfiguration {Id = sameId};
      Action sut2 = () => { configuration.AddApi(api2); };
      sut2.Should().ThrowExactly<PoezdConfigurationException>();
    }
  }
}
