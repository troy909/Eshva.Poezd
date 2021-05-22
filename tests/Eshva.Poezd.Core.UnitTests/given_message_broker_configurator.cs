#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using FluentAssertions;
using Xunit;

#endregion

namespace Eshva.Poezd.Core.UnitTests
{
  public class given_message_broker_configurator
  {
    [Fact]
    public void when_id_set_it_should_be_set_in_configuration()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      const string expected = "id";
      sut.WithId(expected).Should().BeSameAs(sut);
      configuration.Id.Should().Be(expected);
    }

    [Fact]
    public void when_id_set_more_than_once_it_should_fail()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      const string expected = "id";

      Action sut = () => configurator.WithId(expected);

      sut.Should().NotThrow();
      configuration.Id.Should().Be(expected);
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    public void when_ingress_set_it_should_be_set_in_configurator()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      sut.Ingress(ingress => { }).Should().BeSameAs(sut);
      configuration.Ingress.Should().NotBeNull();
    }

    [Fact]
    public void when_ingress_set_more_than_once_it_should_fail()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      Action sut = () => configurator.Ingress(ingress => { });

      sut.Should().NotThrow();
      configuration.Ingress.Should().NotBeNull();
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_ingress_set_and_without_ingress_called_it_should_fail()
    {
      var configurator = new MessageBrokerConfigurator(new MessageBrokerConfiguration());
      Action setIngress = () => configurator.Ingress(ingress => { });
      Action setWithoutIngress = () => configurator.Ingress(ingress => { });

      setIngress.Should().NotThrow();
      EnsureSecondCallOfConfigurationMethodFails(setWithoutIngress);

      configurator = new MessageBrokerConfigurator(new MessageBrokerConfiguration());
      setWithoutIngress.Should().NotThrow();
      EnsureSecondCallOfConfigurationMethodFails(setIngress);
    }

    [Fact]
    public void when_set_without_ingress_more_than_once_it_should_fail()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      Action sut = () => configurator.WithoutIngress();

      sut.Should().NotThrow();
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    public void when_set_without_ingress_it_should_not_be_allowed_to_get_ingress()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      Func<object> sut = () => configuration.Ingress;

      configurator.WithoutIngress();

      sut.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    public void when_egress_set_it_should_be_set_in_configurator()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      sut.Egress(egress => { }).Should().BeSameAs(sut);
      configuration.Egress.Should().NotBeNull();
    }

    [Fact]
    public void when_egress_set_more_than_once_it_should_fail()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      Action sut = () => configurator.Egress(egress => { });

      sut.Should().NotThrow();
      configuration.Egress.Should().NotBeNull();
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_egress_set_and_without_ingress_called_it_should_fail()
    {
      var configurator = new MessageBrokerConfigurator(new MessageBrokerConfiguration());
      Action setEgress = () => configurator.Egress(egress => { });
      Action setWithoutEgress = () => configurator.Egress(ingress => { });

      setEgress.Should().NotThrow();
      EnsureSecondCallOfConfigurationMethodFails(setWithoutEgress);

      configurator = new MessageBrokerConfigurator(new MessageBrokerConfiguration());
      setWithoutEgress.Should().NotThrow();
      EnsureSecondCallOfConfigurationMethodFails(setEgress);
    }

    [Fact]
    public void when_set_without_egress_more_than_once_it_should_fail()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      Action sut = () => configurator.WithoutEgress();

      sut.Should().NotThrow();
      EnsureSecondCallOfConfigurationMethodFails(sut);
    }

    [Fact]
    public void when_set_without_egress_it_should_not_be_allowed_to_get_egress()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      Func<object> sut = () => configuration.Egress;

      configurator.WithoutEgress();

      sut.Should().ThrowExactly<InvalidOperationException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
    public void when_constructed_with_null_as_configuration_it_should_fail()
    {
      Action sut = () => new MessageBrokerConfigurator(configuration: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_set_invalid_value_as_id_it_should_fail()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      var id = "id";

      Action sut = () => configurator.WithId(id);

      id = null;
      sut.Should().ThrowExactly<ArgumentNullException>();
      id = string.Empty;
      sut.Should().ThrowExactly<ArgumentNullException>();
      id = WhitespaceString;
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_set_null_as_ingress_it_should_fail()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      Action sut = () => configurator.Ingress(configurator: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    [Fact]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_set_null_as_egress_it_should_fail()
    {
      var configuration = new MessageBrokerConfiguration();
      var configurator = new MessageBrokerConfigurator(configuration);
      Action sut = () => configurator.Egress(configurator: null);
      sut.Should().ThrowExactly<ArgumentNullException>();
    }

    private static void EnsureSecondCallOfConfigurationMethodFails(Action sut)
    {
      sut.Should().ThrowExactly<PoezdConfigurationException>().Which.Message.Should().Contain(
        "more than once",
        "configuration method should complain about it called twice with exception");
    }

    private const string WhitespaceString = " \n\t";
  }
}
