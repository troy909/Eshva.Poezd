#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public class BrokerIngress : IBrokerIngress
  {
    /// <summary>
    /// Construct a new instance of message broker.
    /// </summary>
    /// <param name="configuration">
    /// The message broker configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public BrokerIngress(
      [NotNull] IngressConfiguration configuration,
      [NotNull] IServiceProvider serviceProvider)
    {
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      Driver = configuration.Driver ?? throw new ArgumentNullException($"{nameof(configuration)}.{nameof(configuration.Driver)}");
      PublicApis = configuration.PublicApis.Select(api => new IngressPublicApi(api, serviceProvider)).ToList().AsReadOnly();
      _queueNameMatcher = (IQueueNameMatcher) serviceProvider.GetService(configuration.QueueNameMatcherType);
      EnterPipeFitter = GetEnterPipeFitter(serviceProvider);
      ExitPipeFitter = GetExitPipeFitter(serviceProvider);
    }

    /// <inheritdoc />
    public IngressConfiguration Configuration { get; }

    /// <inheritdoc />
    public IIngressDriver Driver { get; }

    /// <inheritdoc />
    public IReadOnlyCollection<IIngressPublicApi> PublicApis { get; }

    /// <inheritdoc />
    public IPipeFitter EnterPipeFitter { get; }

    /// <inheritdoc />
    public IPipeFitter ExitPipeFitter { get; }

    /// <summary>
    /// Gets public API by queue name.
    /// </summary>
    /// <param name="queueName">
    /// Queue name that should belong to one of public APIs bound to this broker.
    /// </param>
    /// <returns>
    /// Public API to which queue name belongs or a stab public API for an unknown queue name.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Queue name is null, an empty or a whitespace string.
    /// </exception>
    public IIngressPublicApi GetApiByQueueName(string queueName)
    {
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
      var publicApi = PublicApis.FirstOrDefault(
        api => api.GetQueueNamePatterns()
          .Any(queueNamePattern => _queueNameMatcher.DoesMatch(queueName, queueNamePattern)));
      return publicApi ?? IngressPublicApi.Empty; // TODO: Do I need an empty API here at all?
    }

    public void Initialize(IMessageRouter messageRouter, string brokerId)
    {
      if (messageRouter == null) throw new ArgumentNullException(nameof(messageRouter));
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));

      Driver.Initialize(
        messageRouter,
        brokerId,
        _serviceProvider);
    }

    public Task StartConsumeMessages(IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken = default)
    {
      if (queueNamePatterns == null) throw new ArgumentNullException(nameof(queueNamePatterns));

      return Driver.StartConsumeMessages(queueNamePatterns, cancellationToken);
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
          $"Can not get instance of the message broker ingress enter pipe fitter of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private IPipeFitter GetExitPipeFitter(IServiceProvider serviceProvider)
    {
      return (IPipeFitter) serviceProvider.GetService(
        Configuration.ExitPipeFitterType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker ingress exit pipe fitter of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private readonly IQueueNameMatcher _queueNameMatcher;

    [NotNull]
    private readonly IServiceProvider _serviceProvider;
  }
}
