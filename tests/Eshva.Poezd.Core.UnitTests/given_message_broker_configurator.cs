#region Usings

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
  }
}
