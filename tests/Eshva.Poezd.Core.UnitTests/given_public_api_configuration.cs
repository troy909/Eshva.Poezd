#region Usings

using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_public_api_configuration
  {
    [Fact]
    public void when_all_required_properties_set_it_should_be_validated()
    {
      var sut = ConfigurationTests.CreatePublicApiConfiguration();

      sut.Validate().Should().NotBeNull().And.Subject.Should().BeEmpty("there is no errors in the configuration");
    }

    [Fact]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      ConfigurationTests.CreatePublicApiConfigurationWithout(configuration => configuration.QueueNamePatternsProviderType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreatePublicApiConfigurationWithout(configuration => configuration.Id = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreatePublicApiConfigurationWithout(configuration => configuration.HandlerRegistryType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreatePublicApiConfigurationWithout(configuration => configuration.IngressPipeFitterType = null)
        .Validate().Should().HaveCount(expected: 1);
      ConfigurationTests.CreatePublicApiConfigurationWithout(configuration => configuration.MessageTypesRegistryType = null)
        .Validate().Should().HaveCount(expected: 1);
    }
  }
}
