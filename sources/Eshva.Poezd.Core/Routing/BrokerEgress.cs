#region Usings

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public class BrokerEgress : IBrokerEgress
  {
    public BrokerEgress(
      [NotNull] BrokerEgressConfiguration configuration,
      [NotNull] IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      Driver = configuration.Driver ?? throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.Driver)}");
      PublicApis = configuration.PublicApis.Select(api => new EgressPublicApi(api, serviceProvider)).ToList().AsReadOnly();
      EnterPipeFitter = GetEnterPipeFitter(serviceProvider);
      ExitPipeFitter = GetExitPipeFitter(serviceProvider);
    }

    /// <inheritdoc />
    public BrokerEgressConfiguration Configuration { get; }

    /// <inheritdoc />
    public IBrokerEgressDriver Driver { get; }

    /// <inheritdoc />
    public IPipeFitter EnterPipeFitter { get; }

    /// <inheritdoc />
    public IPipeFitter ExitPipeFitter { get; }

    /// <inheritdoc />
    public ReadOnlyCollection<EgressPublicApi> PublicApis { get; }

    public void Initialize(IMessageRouter messageRouter, string brokerId)
    {
      var logger = (ILogger<IBrokerEgressDriver>) _serviceProvider.GetService(typeof(ILogger<IBrokerEgressDriver>));
      Driver.Initialize(brokerId, logger);
    }

    public Task Publish(
      [NotNull] byte[] key,
      [NotNull] byte[] payload,
      [NotNull] IReadOnlyDictionary<string, string> metadata,
      [NotNull] IReadOnlyCollection<string> queueNames)
    {
      if (key == null) throw new ArgumentNullException(nameof(key));
      if (payload == null) throw new ArgumentNullException(nameof(payload));
      if (metadata == null) throw new ArgumentNullException(nameof(metadata));
      if (queueNames == null) throw new ArgumentNullException(nameof(queueNames));

      return Driver.Publish(
        key,
        payload,
        metadata,
        queueNames);
    }

    public void Dispose()
    {
      Driver.Dispose();
    }

    private IPipeFitter GetEnterPipeFitter(IServiceProvider serviceProvider)
    {
      return (IPipeFitter) serviceProvider.GetService(
        Configuration.EnterPipeFitterType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker egress enter pipe fitter of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private IPipeFitter GetExitPipeFitter(IServiceProvider serviceProvider)
    {
      return (IPipeFitter) serviceProvider.GetService(
        Configuration.ExitPipeFitterType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker egress exit pipe fitter of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private readonly IServiceProvider _serviceProvider;
  }
}
