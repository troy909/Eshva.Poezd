#region Usings

using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Routing;

#endregion

namespace Eshva.Poezd.Adapter.EventStoreDB.Ingress
{
  public class BrokerIngressEventStoreDbDriver : IBrokerIngressDriver
  {
    public BrokerIngressEventStoreDbDriver(
      BrokerIngressEventStoreDbDriverConfiguration driverConfiguration,
      IStreamSubscriptionRegistry streamSubscriptionRegistry)
    {
      _driverConfiguration = driverConfiguration;
      _streamSubscriptionRegistry = streamSubscriptionRegistry;
    }

    public void Initialize(
      IBrokerIngress brokerIngress,
      IEnumerable<IIngressApi> apis,
      IDiContainerAdapter serviceProvider)
    {
      // TODO: Initialize the driver.
      if (_isInitialized) throw new PoezdOperationException("EventStoreDB ingress driver is already initialized.");

      _brokerIngress = brokerIngress;
      _apis = apis;
      _serviceProvider = serviceProvider;

      GetRequiredServices();
      CreateAndRegisterStreamSubscriptions();

      _isInitialized = true;
    }

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default) =>
      // TODO: Subscribe to subscription groups.
      Task.CompletedTask;

    public void Dispose()
    {
      // TODO: Dispose or close all subscriptions.
    }

    private void GetRequiredServices()
    {
      _headerValueCodecType = _serviceProvider.GetService<IHeaderValueCodec>(_driverConfiguration.HeaderValueCodecType);
      _apiStreamSubscriptionFactory =
        _serviceProvider.GetService<IStreamSubscriptionFactory>(_driverConfiguration.StreamSubscriptionFactoryType);
    }

    private void CreateAndRegisterStreamSubscriptions()
    {
      foreach (var api in _apis)
      {
        var concreteMethod = CreateAndRegisterStreamSubscriptionsMethod.MakeGenericMethod(api.MessageKeyType, api.MessagePayloadType);
        concreteMethod.Invoke(this, new object[] {api});
      }
    }

    private void CreateAndRegisterStreamSubscriptions<TKey, TValue>(IIngressApi api) =>
      _streamSubscriptionRegistry.Add(
        api,
        _apiStreamSubscriptionFactory.Create<TKey, TValue>(_driverConfiguration.ConnectionConfiguration, api));

    private readonly BrokerIngressEventStoreDbDriverConfiguration _driverConfiguration;
    private readonly IStreamSubscriptionRegistry _streamSubscriptionRegistry;
    private IEnumerable<IIngressApi> _apis;
    private IStreamSubscriptionFactory _apiStreamSubscriptionFactory;
    private IBrokerIngress _brokerIngress;
    private IHeaderValueCodec _headerValueCodecType;
    private bool _isInitialized;
    private IDiContainerAdapter _serviceProvider;

    private static readonly MethodInfo CreateAndRegisterStreamSubscriptionsMethod =
      typeof(BrokerIngressEventStoreDbDriver).GetMethod(
        nameof(CreateAndRegisterStreamSubscriptions),
        BindingFlags.Instance | BindingFlags.NonPublic);
  }

  internal interface IStreamSubscriptionFactory
  {
    IStreamSubscription Create<TKey, TValue>(EventStoreDbConnectionConfiguration connectionConfiguration, IIngressApi api);
  }
}
