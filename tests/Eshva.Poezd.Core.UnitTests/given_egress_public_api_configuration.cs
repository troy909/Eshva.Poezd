#region Usings

using System.Reflection;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_egress_public_api_configuration
  {
    [Fact]
    public void when_all_required_properties_set_it_should_be_valid()
    {
      var sut = ConfigurationTests.CreateEgressPublicApiConfiguration();

      sut.Validate().Should().NotBeNull().And.Subject.Should().BeEmpty("there is no errors in the configuration");
    }

    [Fact]
    public void when_validating_it_should_validate_expected_number_of_properties()
    {
      const int expectedNumberOfValidatingProperties = 3;

      var properties = typeof(EgressPublicApiConfiguration).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      properties.Should().HaveCount(
        expectedNumberOfValidatingProperties,
        $"Seams like you've added new properties to {nameof(EgressPublicApiConfiguration)}. " +
        $"Update its ValidateItself method to test them or update {nameof(expectedNumberOfValidatingProperties)} const value.");
    }

    [Fact]
    public void when_getting_empty_api_configuration_it_should_return_valid_instance()
    {
      EgressPublicApiConfiguration.Empty.Validate().Should().BeEmpty("empty instance should be valid");
    }

    [Fact]
    public void when_getting_empty_api_configuration_few_times_it_should_return_same_instance()
    {
      EgressPublicApiConfiguration.Empty.Should().BeSameAs(EgressPublicApiConfiguration.Empty, "it always should be the same instance");
    }

    [Fact]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      ConfigurationTests.CreateEgressPublicApiConfigurationWithout(configuration => configuration.Id = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateEgressPublicApiConfigurationWithout(configuration => configuration.PipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateEgressPublicApiConfigurationWithout(configuration => configuration.MessageTypesRegistryType = null)
        .Validate().Should().HaveCount(expected: 1);
    }
  }
}
