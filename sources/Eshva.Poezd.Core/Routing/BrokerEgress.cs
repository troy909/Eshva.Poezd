#region Usings

using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// The message broker egress.
  /// </summary>
  internal class BrokerEgress : IBrokerEgress
  {
    /// <summary>
    /// Constructs a new instance of message broker egress.
    /// </summary>
    /// <param name="brokerId">
    /// The message broker ID.
    /// </param>
    /// <param name="configuration">
    /// The message broker egress configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// TODO: Eliminate this parameter.
    /// <param name="clock">
    /// The current time service.
    /// </param>
    public BrokerEgress(
      string brokerId,
      [NotNull] BrokerEgressConfiguration configuration,
      [NotNull] IDiContainerAdapter serviceProvider,
      [NotNull] IClock clock)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _clock = clock ?? throw new ArgumentNullException(nameof(clock));
      _brokerId = brokerId;
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      Driver = configuration.Driver ?? throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.Driver)}");
      Apis = configuration.Apis.Select(api => new EgressApi(api, serviceProvider)).Cast<IEgressApi>().ToList().AsReadOnly();
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
    public ReadOnlyCollection<IEgressApi> Apis { get; }

    /// <inheritdoc />
    public void Initialize() =>
      Driver.Initialize(
        _brokerId,
        _serviceProvider.GetService<ILogger<IBrokerEgressDriver>>(),
        _clock,
        Apis,
        _serviceProvider);

    /// <inheritdoc />
    public Task Publish(MessagePublishingContext context, CancellationToken cancellationToken) =>
      Driver.Publish(context, cancellationToken);

    /// <inheritdoc />
    public void Dispose() => Driver.Dispose();

    private IPipeFitter GetEnterPipeFitter(IDiContainerAdapter serviceProvider) =>
      serviceProvider.GetService<IPipeFitter>(
        Configuration.EnterPipeFitterType,
        exception => new PoezdConfigurationException(
          $"Can not get instance of the message broker egress enter pipe fitter of type '{Configuration.EnterPipeFitterType}'. " +
          "You should register this type in DI-container.",
          exception));

    private IPipeFitter GetExitPipeFitter(IDiContainerAdapter serviceProvider) =>
      serviceProvider.GetService<IPipeFitter>(
        Configuration.ExitPipeFitterType,
        exception => new PoezdConfigurationException(
          $"Can not get instance of the message broker egress exit pipe fitter of type '{Configuration.ExitPipeFitterType}'. " +
          "You should register this type in DI-container.",
          exception));

    private readonly string _brokerId;

    private readonly IClock _clock;
    private readonly IDiContainerAdapter _serviceProvider;
  }
}
