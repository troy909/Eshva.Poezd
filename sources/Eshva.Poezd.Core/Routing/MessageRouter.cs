#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion


namespace Eshva.Poezd.Core.Routing
{
  public sealed class MessageRouter : IMessageRouter
  {
    internal MessageRouter(
      [NotNull] PoezdConfiguration configuration,
      [NotNull] IServiceProvider serviceProvider)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _logger = (ILogger<MessageRouter>)_serviceProvider.GetService(typeof(ILogger<MessageRouter>)) ??
                throw new ArgumentException($"Can not get {nameof(ILogger<MessageRouter>)} implementation from the service provider.");
    }

    public Task RouteIncomingMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] brokerPayload,
      IReadOnlyDictionary<string, string> brokerMetadata,
      string messageId)
    {
      if (brokerPayload == null) throw new ArgumentNullException(nameof(brokerPayload));
      if (brokerMetadata == null) throw new ArgumentNullException(nameof(brokerMetadata));
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentException(NotWhitespace, nameof(brokerId));
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException(NotWhitespace, nameof(queueName));
      if (string.IsNullOrWhiteSpace(messageId)) throw new ArgumentException(NotWhitespace, nameof(messageId));

      var brokerConfiguration = _configuration.Brokers.Single(
        configuration => configuration.Id.Equals(brokerId, StringComparison.InvariantCultureIgnoreCase));
      var brokerAdapter = GetBrokerAdapter(brokerConfiguration);
      if (brokerAdapter == null)
      {
        return Task.CompletedTask; // TODO: Decide what to log, what to throw, where to do it better.
      }

      var externalServiceConfiguration = GetExternalServicesConfiguration(brokerConfiguration, queueName);

      var externalServiceAdapter = GetExternalServicesAdapter(externalServiceConfiguration);
      if (externalServiceAdapter == null)
      {
        return Task.CompletedTask;
      }

      var pipeline = new MessageHandlingPipeline()
                     .Append(brokerAdapter.GetPipelineSteps())
                     .Append(externalServiceAdapter.GetPipelineSteps())
                     .Append(GetMessageHandlingSteps());

      var messageHandlingContext = new MessageHandlingContext();
      messageHandlingContext.Set(ContextKeys.Broker.Id, brokerId)
                            .Set(ContextKeys.Broker.MessageMetadata, brokerMetadata)
                            .Set(ContextKeys.Broker.MessagePayload, brokerPayload)
                            .Set(ContextKeys.Broker.QueueName, queueName)
                            .Set(ContextKeys.Broker.ReceivedOnUtc, receivedOnUtc)
                            .Set(ContextKeys.Broker.Configuration, brokerConfiguration)
                            .Set(ContextKeys.ExternalService.Id, externalServiceConfiguration.Id)
                            .Set(ContextKeys.ExternalService.Name, externalServiceConfiguration.Name)
                            .Set(ContextKeys.ExternalService.Adapter, externalServiceAdapter)
                            .Set(ContextKeys.ExternalService.Configuration, externalServiceConfiguration);

      return pipeline.Execute(messageHandlingContext);
    }

    public static PoezdConfiguration Configure([NotNull] Action<PoezdConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var poezdConfigurator = new PoezdConfigurator();
      configurator(poezdConfigurator);
      return poezdConfigurator.Configuration;
    }

    private IMessageBrokerAdapter GetBrokerAdapter(MessageBrokerConfiguration brokerConfiguration)
    {
      var brokerAdapter = _serviceProvider.GetService(brokerConfiguration.AdapterType);
      if (brokerAdapter != null)
      {
        return (IMessageBrokerAdapter)brokerAdapter;
      }

      _logger.LogError("Message broker adapter isn't configured. {TODO: add more info}");
      return null;
    }

    private IEnumerable<IStep> GetMessageHandlingSteps()
    {
      yield break;
    }

    private ExternalServiceConfiguration GetExternalServicesConfiguration(MessageBrokerConfiguration brokerConfiguration, string queueName)
    {
      var queueNameMatcher = (IQueueNameMatcher)_serviceProvider.GetService(brokerConfiguration.QueueNameMatcherType);
      var externalServiceConfiguration = brokerConfiguration.ExternalServices.SingleOrDefault(
        configuration => configuration.QueueNamePatterns
                                      .Any(queueNamePattern => queueNameMatcher.IsMatch(queueName, queueNamePattern)));
      if (externalServiceConfiguration != null)
      {
        return externalServiceConfiguration;
      }

      _logger.LogDebug(
        $"The queue '{queueName}' for '{brokerConfiguration.Name}' broker isn't configured " +
        "but a message received from this queue.");
      return null;
    }

    private IExternalServiceAdapter GetExternalServicesAdapter(ExternalServiceConfiguration externalServiceConfiguration)
    {
      var externalServiceAdapter = _serviceProvider.GetService(externalServiceConfiguration.AdapterType);
      if (externalServiceAdapter != null)
      {
        return (IExternalServiceAdapter)externalServiceAdapter;
      }

      _logger.LogError(
        $"Type of the adapter for '{externalServiceConfiguration.Name}' isn't configured. " +
        "Use {TODO: class and method name} method to set the adapter type." + "" +
        "{TODO: add a code example.}");
      return null;
    }

    private readonly ILogger<MessageRouter> _logger;
    private readonly PoezdConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
