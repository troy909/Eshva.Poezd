#region Usings

using System;
using System.Linq;
using Eshva.Poezd.Adapter.EventStoreDB.Ingress;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;
using FluentAssertions;
using Moq;
using Xunit;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.UnitTests
{
  public class given_broker_ingress_event_store_db_driver
  {
    [Fact]
    public void when_initialize_more_than_once_it_should_fail()
    {
      var driver = new BrokerIngressEventStoreDbDriver(MakeDriverConfiguration(), Mock.Of<IStreamSubscriptionRegistry>());
      Action sut = () => driver.Initialize(
        Mock.Of<IBrokerIngress>(),
        Enumerable.Empty<IIngressApi>(),
        MakeServiceProviderMock().Object);

      sut.Should().NotThrow("the first initialization should be successive");
      sut.Should().ThrowExactly<PoezdOperationException>().Which.Message.Should().Contain(
        "already initialized",
        "it should not be possible to initialize driver more than once");
    }

    [Fact]
    public void when_initialize_it_should_create_subscription_for_each_api()
    {
      var subscriptionCount = 0;
      var subscriptionRegistryMock = new Mock<IStreamSubscriptionRegistry>();
      subscriptionRegistryMock
        .Setup(registry => registry.Add(It.IsAny<IIngressApi>(), It.IsAny<IStreamSubscription>()))
        .Callback(() => subscriptionCount++);
      var sut = new BrokerIngressEventStoreDbDriver(MakeDriverConfiguration(), subscriptionRegistryMock.Object);

      sut.Initialize(
        Mock.Of<IBrokerIngress>(),
        new[] {MakeApi<int, byte[]>(), MakeApi<int, string>()},
        MakeServiceProviderMock().Object);

      subscriptionCount.Should().Be(expected: 2, "driver was initialized with 2 subscriptions from 2 APIs");
    }

    private static BrokerIngressEventStoreDbDriverConfiguration MakeDriverConfiguration() =>
      new BrokerIngressEventStoreDbDriverConfiguration();

    private static Mock<IDiContainerAdapter> MakeServiceProviderMock()
    {
      var mock = new Mock<IDiContainerAdapter>();
      // TODO: Add required service mocks.
      // mock.Setup(provider => provider.GetService(typeof(TestDeserializerFactory))).Returns(new TestDeserializerFactory());
      return mock;
    }

    private static IIngressApi MakeApi<TKey, TValue>()
    {
      var mock = new Mock<IIngressApi>();
      mock.SetupGet(api => api.MessageKeyType).Returns(typeof(TKey));
      mock.SetupGet(api => api.MessagePayloadType).Returns(typeof(TValue));
      return mock.Object;
    }
  }
}
