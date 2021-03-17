#region Usings

using System;
using System.Diagnostics.CodeAnalysis;
using Eshva.Poezd.Adapter.Kafka.Ingress;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.Kafka.UnitTests
{
  public class given_default_consumer_registry
  {
    [Fact]
    public void when_add_consumer_with_valid_arguments_it_should_be_possible_to_get_it_back()
    {
      var sut = new DefaultConsumerRegistry();
      var expectedConsumer = Mock.Of<IApiConsumer<int, string>>();
      var api = Mock.Of<IIngressApi>();
      sut.Add(api, expectedConsumer);

      sut.Get<int, string>(api).Should().BeSameAs(expectedConsumer, "the same consumer should be returned");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    public void when_add_consumer_with_invalid_arguments_it_should_fail()
    {
      var registry = new DefaultConsumerRegistry();
      var api = Mock.Of<IIngressApi>();
      var consumer = Mock.Of<IApiConsumer<int, string>>();

      Action sut = () => registry.Add(api, consumer);

      api = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("api"));
      api = Mock.Of<IIngressApi>();

      consumer = null;
      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("consumer"));
    }

    [Fact]
    public void when_add_consumer_for_same_api_it_should_fail()
    {
      var registry = new DefaultConsumerRegistry();
      var api = Mock.Of<IIngressApi>();

      Action sut = () => registry.Add(api, Mock.Of<IApiConsumer<int, string>>());

      sut.Should().NotThrow("the first time it should be added");
      sut.Should().ThrowExactly<PoezdConfigurationException>().Where(
        exception => exception.Message.Contains("already registered"),
        "it should not be possible to add a consumer for same API twice");
    }

    [Fact]
    [SuppressMessage("ReSharper", "AccessToModifiedClosure")]
    [SuppressMessage("ReSharper", "AssignNullToNotNullAttribute")]
    public void when_get_consumer_with_invalid_arguments_it_should_fail()
    {
      var registry = new DefaultConsumerRegistry();
      Action sut = () => registry.Get<int, string>(api: null);

      sut.Should().ThrowExactly<ArgumentNullException>().Where(exception => exception.ParamName.Equals("api"));
    }

    [Fact]
    public void when_get_consumer_for_not_registered_api_it_should_fail()
    {
      var registry = new DefaultConsumerRegistry();
      Action sut = () => registry.Get<int, string>(Mock.Of<IIngressApi>());

      sut.Should().ThrowExactly<ArgumentException>().Where(
        exception => exception.ParamName.Equals("api"),
        "it's not possible to get consumer for not registered API");
    }

    [Fact]
    public void when_dispose_it_should_dispose_all_registered_consumers()
    {
      var disposed = 0;
      var sut = new DefaultConsumerRegistry();
      var consumer1Mock = new Mock<IApiConsumer<int, string>>();
      consumer1Mock.Setup(consumer => consumer.Dispose()).Callback(() => disposed++);
      var consumer2Mock = new Mock<IApiConsumer<int, string>>();
      consumer2Mock.Setup(consumer => consumer.Dispose()).Callback(() => disposed++);
      sut.Add(Mock.Of<IIngressApi>(), consumer1Mock.Object);
      sut.Add(Mock.Of<IIngressApi>(), consumer2Mock.Object);

      sut.Dispose();

      disposed.Should().Be(expected: 2, "all registered consumers should be disposed");
    }
  }
}
