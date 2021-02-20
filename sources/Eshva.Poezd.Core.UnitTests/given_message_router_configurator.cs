#region Usings

using System;
using System.Linq;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_message_router_configurator
  {
    [Fact]
    public void when_message_broker_added_it_should_be_added_into_configuration()
    {
      var sut = new MessageRouterConfigurator();
      const string expectedId = "id";
      sut.AddMessageBroker(broker => broker.WithId(expectedId));
      sut.Configuration.Brokers.Single(broker => broker.Id.Equals(expectedId)).Should().NotBeNull("broker should be added");
    }

    [Fact]
    public void when_null_added_as_message_broker_it_should_fail()
    {
      var configurator = new MessageRouterConfigurator();
      // ReSharper disable once AssignNullToNotNullAttribute - it's a test against null.
      Action sut = () => configurator.AddMessageBroker(configurator: null);
      sut.Should().Throw<ArgumentNullException>();
    }
  }
}
