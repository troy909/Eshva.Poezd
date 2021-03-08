#region Usings

using System.Reflection;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_ingress_api_configuration
  {
    [Fact]
    public void when_all_required_properties_set_it_should_be_valid()
    {
      var sut = ConfigurationTests.CreateIngressApiConfiguration();

      sut.Validate().Should().NotBeNull().And.Subject.Should().BeEmpty("there is no errors in the configuration");
    }

    [Fact]
    public void when_validating_it_should_validate_expected_number_of_properties()
    {
      const int expectedNumberOfValidatingProperties = 5;

      var properties = typeof(IngressApiConfiguration).GetProperties(BindingFlags.Public | BindingFlags.Instance);
      properties.Should().HaveCount(
        expectedNumberOfValidatingProperties,
        $"Seams like you've added new properties to {nameof(IngressApiConfiguration)}. " +
        $"Update its ValidateItself method to test them or update {nameof(expectedNumberOfValidatingProperties)} const value.");
    }

    [Fact]
    public void when_getting_empty_api_configuration_it_should_return_valid_instance()
    {
      IngressApiConfiguration.Empty.Validate().Should().BeEmpty("empty instance should be valid");
    }

    [Fact]
    public void when_getting_empty_api_configuration_few_times_it_should_return_same_instance()
    {
      IngressApiConfiguration.Empty.Should().BeSameAs(IngressApiConfiguration.Empty, "it always should be the same instance");
    }

    [Fact]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      ConfigurationTests.CreateIngressApiConfigurationWithout(configuration => configuration.QueueNamePatternsProviderType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateIngressApiConfigurationWithout(configuration => configuration.Id = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateIngressApiConfigurationWithout(configuration => configuration.HandlerRegistryType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateIngressApiConfigurationWithout(configuration => configuration.PipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreateIngressApiConfigurationWithout(configuration => configuration.MessageTypesRegistryType = null)
        .Validate().Should().HaveCount(expected: 1);
    }
  }
}
