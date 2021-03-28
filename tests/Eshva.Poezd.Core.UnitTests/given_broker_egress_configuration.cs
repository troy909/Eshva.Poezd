#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_broker_egress_configuration
  {
    [Fact]
    public void when_all_required_properties_set_it_should_be_validated()
    {
      var sut = ConfigurationTests.CreateBrokerEgressConfiguration();

      sut.Validate().Should().NotBeNull().And.Subject.Should().BeEmpty("there is no errors in the configuration");
    }

    [Fact]
    public void when_validating_it_should_validate_expected_number_of_properties()
    {
      const int expectedNumberOfValidatingProperties = 5;
      const int numberOfNotValidatingPublicProperties = 0;

      var validatingProperties = typeof(BrokerEgressConfiguration).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      validatingProperties.Should().HaveCount(
        expectedNumberOfValidatingProperties + numberOfNotValidatingPublicProperties,
        $"seams like you've added new properties to {nameof(BrokerEgressConfiguration)}. " +
        "Update its ValidateItself and GetChildConfigurations methods to test them or " +
        $"update {nameof(expectedNumberOfValidatingProperties)} const value.");
    }

    [Fact]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      ConfigurationTests.CreateBrokerEgressConfigurationWithout(configuration => configuration.Driver = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerEgressConfigurationWithout(configuration => configuration.DriverConfiguration = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerEgressConfigurationWithout(configuration => configuration.EnterPipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerEgressConfigurationWithout(configuration => configuration.ExitPipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateBrokerEgressConfiguration(shouldAddApis: false)
        .Validate().Should().HaveCount(expected: 1);
    }

    [Fact]
    public void when_same_api_added_second_time_it_should_fail()
    {
      var configuration = new BrokerEgressConfiguration();
      var api = new EgressApiConfiguration();
      Action sut = () => configuration.AddApi(api);
      sut.Should().NotThrow();
      sut.Should().ThrowExactly<PoezdConfigurationException>();
    }

    [Fact]
    public void when_api_with_same_id_added_it_should_fail()
    {
      var configuration = new BrokerEgressConfiguration();

      const string sameId = "same id";
      var api1 = new EgressApiConfiguration {Id = sameId};
      Action sut1 = () => configuration.AddApi(api1);
      sut1.Should().NotThrow();

      var api2 = new EgressApiConfiguration {Id = sameId};
      Action sut2 = () => configuration.AddApi(api2);
      sut2.Should().ThrowExactly<PoezdConfigurationException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_add_null_as_api_it_should_fail()
    {
      var configuration = new BrokerEgressConfiguration();
      Action sut = () => configuration.AddApi(configuration: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    public void when_validating_and_driver_configuration_contains_errors_it_should_fail()
    {
      var sut = ConfigurationTests.CreateBrokerEgressConfiguration();
      const string expectedError = "broken driver configuration";
      ((TestBrokerEgressDriverConfiguration) sut.DriverConfiguration).ErrorToReport = expectedError;
      sut.Validate().Should().HaveCount(expected: 1).And.Contain(expectedError, "driver configuration errors should be reported");
    }
  }
}
