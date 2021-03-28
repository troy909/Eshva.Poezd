#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.UnitTests.TestSubjects;
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
    public void when_ingress_set_it_should_be_set_in_configurator()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      sut.Ingress(ingress => ingress.WithEnterPipeFitter<Service1PipeFitter>()).Should().BeSameAs(sut);
      configuration.Ingress.EnterPipeFitterType.Should().Be<Service1PipeFitter>();
    }

    [Fact]
    public void when_egress_set_it_should_be_set_in_configurator()
    {
      var configuration = new MessageBrokerConfiguration();
      var sut = new MessageBrokerConfigurator(configuration);
      sut.Egress(egress => egress.WithEnterPipeFitter<Service1PipeFitter>()).Should().BeSameAs(sut);
      configuration.Egress.EnterPipeFitterType.Should().Be<Service1PipeFitter>();
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

    private const string WhitespaceString = " \n\t";
  }
}
