#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_message_router_configuration
  {
    [Fact]
    public void when_all_required_properties_set_it_should_be_validated()
    {
      var sut = ConfigurationTests.CreateMessageRouterConfiguration();

      sut.Validate().Should().NotBeNull().And.Subject.Should().BeEmpty("there is no errors in the configuration");
    }

    [Fact]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      new MessageRouterConfiguration().Validate().Should().HaveCount(expected: 1);
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_add_null_as_broker_it_should_fail()
    {
      var configuration = ConfigurationTests.CreateMessageRouterConfiguration();
      Action sut = () => configuration.AddBroker(configuration: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }
  }
}
