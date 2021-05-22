#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
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
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_some_required_property_not_set_it_should_be_not_validated()
    {
      ConfigurationTests.CreateMessageBrokerConfiguration().With(configuration => configuration.Id = null)
        .Validate().Should().HaveCount(expected: 1);

      Action sutIngress = () => ConfigurationTests.CreateMessageBrokerConfiguration().Ingress = null;
      sutIngress.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("value"));

      Action sutEgress = () => ConfigurationTests.CreateMessageBrokerConfiguration().Egress = null;
      sutEgress.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("value"));
    }
  }
}
