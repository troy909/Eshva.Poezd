#region Usings

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Configuration;
using JetBrains.Annotations;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// Message broker.
  /// </summary>
  public sealed class MessageBroker : IDisposable
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
    public MessageBroker(
      [NotNull] MessageBrokerConfiguration configuration,
      [NotNull] IServiceProvider serviceProvider)
    {
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      Ingress = new BrokerIngress(configuration.Ingress, serviceProvider);
      Egress = new BrokerEgress(configuration.Egress, serviceProvider);
    }

    public IBrokerIngress Ingress { get; }

    public IBrokerEgress Egress { get; }

    /// <summary>
    /// Gets the message broker ID.
    /// </summary>
    [NotNull]
    public string Id => Configuration.Id;

    /// <summary>
    /// The message broker configuration.
    /// </summary>
    [NotNull]
    public MessageBrokerConfiguration Configuration { get; }

    /// <inheritdoc />
    public void Dispose()
    {
      Ingress.Dispose();
      Egress.Dispose();
    }

    public void Initialize(IMessageRouter messageRouter, string brokerId)
    {
      Ingress.Initialize(messageRouter, brokerId);
      Egress.Initialize(messageRouter, brokerId);
    }

    public Task Publish(
      byte[] key,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      IReadOnlyCollection<string> queueNames)
    {
      return Egress.Publish(
        key,
        payload,
        metadata,
        queueNames);
    }

    public Task StartConsumeMessages([NotNull] IEnumerable<string> queueNamePatterns, CancellationToken cancellationToken =default)
    {
      if (queueNamePatterns == null) throw new ArgumentNullException(nameof(queueNamePatterns));

      return Ingress.StartConsumeMessages(queueNamePatterns, cancellationToken);
    }
  }

  /*
  /// <summary>
  /// Message broker.
  /// </summary>
  public sealed class MessageBroker : IDisposable
  {
    /// <summary>
    /// Construct a new instance of message broker.
    /// </summary>
    /// <param name="driver">
    /// The message broker driver.
    /// </param>
    /// <param name="configuration">
    /// The message broker configuration.
    /// </param>
    /// <param name="serviceProvider">
    /// Service provider.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    public MessageBroker(
      [NotNull] IMessageBrokerDriver driver,
      [NotNull] MessageBrokerConfiguration configuration,
      [NotNull] IServiceProvider serviceProvider)
    {
      if (serviceProvider == null) throw new ArgumentNullException(nameof(serviceProvider));

      Driver = driver ?? throw new ArgumentNullException(nameof(driver));
      Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      PublicApis = configuration.PublicApis.Select(
        apiConfiguration => new PublicApi(apiConfiguration, serviceProvider)).ToList().AsReadOnly();
      _queueNameMatcher = (IQueueNameMatcher) serviceProvider.GetService(configuration.QueueNameMatcherType);
      IngressEnterPipeFitter = GetIngressEnterPipeFitter(serviceProvider);
      IngressExitPipeFitter = GetIngressExitPipeFitter(serviceProvider);
      EgressEnterPipeFitter = GetEgressEnterPipeFitter(serviceProvider);
      EgressExitPipeFitter = GetEgressExitPipeFitter(serviceProvider);
    }

    /// <summary>
    /// Gets the message broker driver.
    /// </summary>
    [NotNull]
    public IMessageBrokerDriver Driver { get; }

    /// <summary>
    /// Gets the message broker ID.
    /// </summary>
    [NotNull]
    public string Id => Configuration.Id;

    /// <summary>
    /// Gets list of public APIs bound to this message broker.
    /// </summary>
    [NotNull]
    public IReadOnlyCollection<IPublicApi> PublicApis { get; }

    /// <summary>
    /// Gets the message broker driver configuration.
    /// </summary>
    [NotNull]
    public object DriverConfiguration => Configuration.DriverConfiguration;

    /// <summary>
    /// The message broker configuration.
    /// </summary>
    [NotNull]
    public MessageBrokerConfiguration Configuration { get; }

    /// <summary>
    /// Gets ingress enter pipe fitter. Configures the very beginning of ingress pipeline.
    /// </summary>
    [NotNull]
    public IPipeFitter IngressEnterPipeFitter { get; }

    /// <summary>
    /// Gets ingress exit pipe fitter. Configures the very end of ingress pipeline.
    /// </summary>
    [NotNull]
    public IPipeFitter IngressExitPipeFitter { get; }

    /// <summary>
    /// Gets egress enter pipe fitter. Configures the very beginning of egress pipeline.
    /// </summary>
    [NotNull]
    public IPipeFitter EgressEnterPipeFitter { get; }

    /// <summary>
    /// Gets egress exit pipe fitter. Configures the very end of egress pipeline.
    /// </summary>
    [NotNull]
    public IPipeFitter EgressExitPipeFitter { get; }

    /// <inheritdoc />
    public void Dispose()
    {
      Driver.Dispose();
    }

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
    public IPublicApi GetApiByQueueName([NotNull] string queueName)
    {
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));
      var publicApi = PublicApis.FirstOrDefault(
        api => api.GetQueueNamePatterns()
          .Any(queueNamePattern => _queueNameMatcher.DoesMatch(queueName, queueNamePattern)));
      return publicApi ?? PublicApi.Empty; // TODO: Do I need an empty API here at all?
    }

    private IPipeFitter GetIngressEnterPipeFitter(IServiceProvider serviceProvider)
    {
      return (IPipeFitter) serviceProvider.GetService(
        Configuration.IngressEnterPipeFitterType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker ingress enter pipe fitter of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private IPipeFitter GetIngressExitPipeFitter(IServiceProvider serviceProvider)
    {
      return (IPipeFitter) serviceProvider.GetService(
        Configuration.IngressExitPipeFitterType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker ingress exit pipe fitter of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private IPipeFitter GetEgressEnterPipeFitter(IServiceProvider serviceProvider)
    {
      return (IPipeFitter) serviceProvider.GetService(
        Configuration.EgressEnterPipeFitterType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker egress enter pipe fitter of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private IPipeFitter GetEgressExitPipeFitter(IServiceProvider serviceProvider)
    {
      return (IPipeFitter) serviceProvider.GetService(
        Configuration.EgressExitPipeFitterType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker egress exit pipe fitter of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private readonly IQueueNameMatcher _queueNameMatcher;
  }
*/
}
