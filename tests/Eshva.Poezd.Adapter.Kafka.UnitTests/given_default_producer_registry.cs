#region Usings

using System;
using Confluent.Kafka;
using Eshva.Poezd.Adapter.Kafka.Egress;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_default_producer_registry
  {
    [Fact]
    public void when_add_producers_for_different_apis_it_should_successfully_register_them()
    {
      var sut = new DefaultProducerRegistry();
      var api1 = Mock.Of<IEgressApi>();
      var producer1 = Mock.Of<IApiProducer>();
      sut.Add(api1, producer1);
      var api2 = Mock.Of<IEgressApi>();
      var producer2 = Mock.Of<IApiProducer>();
      sut.Add(api2, producer2);

      sut.Get(api1).Should().Be(producer1, "this producer registered for this API");
      sut.Get(api2).Should().Be(producer2, "this producer registered for this API");
    }

    [Fact]
    public void when_get_not_registered_producer_it_should_fail()
    {
      var registry = new DefaultProducerRegistry();
      Action sut = () => registry.Get(Mock.Of<IEgressApi>());
      sut.Should().ThrowExactly<ArgumentException>().Where(exception => exception.ParamName.Equals("api"), "API is required");
    }

    [Fact]
    public void when_get_producer_for_unregistered_api_it_should_fail()
    {
      var registry = new DefaultProducerRegistry();
      var apiMock = new Mock<IEgressApi>();
      apiMock.SetupGet(api => api.Id).Returns("id-1");
      Action sut = () => registry.Get(Mock.Of<IEgressApi>());
      sut.Should().ThrowExactly<ArgumentException>()
        .Where(exception => exception.Message.Contains("There is no registered producers for API with ID"), "should fail");
    }

    [Fact]
    public void when_add_producer_for_already_registered_api_it_should_fail()
    {
      var registry = new DefaultProducerRegistry();
      var api = Mock.Of<IEgressApi>();
      Action sut = () => registry.Add(api, Mock.Of<IApiProducer>());
      sut.Should().NotThrow("producer for this API is not registered yet");
      sut.Should().ThrowExactly<PoezdConfigurationException>("producer for this API is registered already");
    }

    [Fact]
    public void when_dispose_it_should_dispose_all_producers()
    {
      var sut = new DefaultProducerRegistry();
      var api1 = Mock.Of<IEgressApi>();
      var producer1 = new Mock<IApiProducer>();
      var disposed = 0;
      producer1.Setup(producer => producer.Dispose()).Callback(() => disposed++);
      sut.Add(api1, producer1.Object);
      var api2 = Mock.Of<IEgressApi>();
      var producer2 = new Mock<IApiProducer>();
      producer2.Setup(producer => producer.Dispose()).Callback(() => disposed++);
      sut.Add(api2, producer2.Object);
      var api3 = Mock.Of<IEgressApi>();
      var producer3 = new Mock<IApiProducer>();
      producer3.Setup(producer => producer.Dispose()).Callback(() => disposed++);
      sut.Add(api3, producer3.Object);

      sut.Dispose();

      disposed.Should().Be(expected: 3, "all producers should be disposed");
    }
  }
}
