#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.Common;
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
      [NotNull] MessageRouterConfiguration configuration,
      [NotNull] IServiceProvider serviceProvider)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
      _logger = (ILogger<MessageRouter>)_serviceProvider.GetService(typeof(ILogger<MessageRouter>)) ??
                throw new ArgumentException($"Can not get {nameof(ILogger<MessageRouter>)} implementation from the service provider.");
    }

    public IReadOnlyCollection<MessageBroker> Brokers => _brokers.AsReadOnly();

    public async Task Start(CancellationToken cancellationToken = default)
    {
      if (_isStarted)
      {
        throw new PoezdOperationException("The router is started already.");
      }

      ValidateConfiguration();

      try
      {
        ConfigureMessageBrokerDrivers();
        foreach (var descriptor in _brokers)
        {
          var queueNamePatterns = descriptor.PublicApis.SelectMany(api => api.QueueNamePatterns);
          await descriptor.Driver.SubscribeToQueues(queueNamePatterns);
          await descriptor.Driver.StartConsumeMessages();
        }
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException(
          "Unable to start message router due an error. Inspect the inner exception for detailed information.",
          exception);
      }

      _isStarted = true;
    }

    private void ValidateConfiguration()
    {
      var configurationErrors = _configuration.Validate().ToList();
      if (configurationErrors.Any())
      {
        var message = new StringBuilder("Unable to start the message router due configuration errors:");
        configurationErrors.ForEach(error => message.AppendLine($"\t* {error}"));

        throw new PoezdConfigurationException(message.ToString());
      }
    }

    public Task RouteIncomingMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] brokerPayload,
      IReadOnlyDictionary<string, string> brokerMetadata)
    {
      if (brokerPayload == null) throw new ArgumentNullException(nameof(brokerPayload));
      if (brokerMetadata == null) throw new ArgumentNullException(nameof(brokerMetadata));
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentException(NotWhitespace, nameof(brokerId));
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException(NotWhitespace, nameof(queueName));

      MessageHandlingPipeline pipeline;
      var messageHandlingContext = new MessageHandlingContext();
      try
      {
        var brokerConfiguration = _configuration.Brokers.Single(
          configuration => configuration.Id.Equals(brokerId, StringComparison.InvariantCultureIgnoreCase));
        var publicApiConfiguration = GetPublicApiConfiguration(brokerConfiguration, queueName);
        pipeline = BuildPipeline(brokerConfiguration, publicApiConfiguration);

        messageHandlingContext.Set(ContextKeys.Broker.Id, brokerId)
                              .Set(ContextKeys.Broker.MessageMetadata, brokerMetadata)
                              .Set(ContextKeys.Broker.MessagePayload, brokerPayload)
                              .Set(ContextKeys.Broker.QueueName, queueName)
                              .Set(ContextKeys.Broker.ReceivedOnUtc, receivedOnUtc)
                              .Set(ContextKeys.Broker.Configuration, brokerConfiguration)
                              .Set(ContextKeys.PublicApi.Id, publicApiConfiguration.Id)
                              .Set(ContextKeys.PublicApi.Configuration, publicApiConfiguration);
      }
      catch (Exception exception)
      {
        _logger.LogError(
          exception,
          "An error occurred during preparing to handle a message. See inner exception to find details about the error.");
        throw;
      }

      try
      {
        return pipeline.Execute(messageHandlingContext);
      }
      catch (Exception exception)
      {
        _logger.LogError(
          exception,
          "An error occurred during message handling. See inner exception to find details about the error.");
        messageHandlingContext.Rollback();
        return Task.CompletedTask;
      }
    }

    public static MessageRouterConfiguration Configure([NotNull] Action<PoezdConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var poezdConfigurator = new PoezdConfigurator();
      configurator(poezdConfigurator);
      return poezdConfigurator.Configuration;
    }

    private void ConfigureMessageBrokerDrivers()
    {
      foreach (var broker in _configuration.Brokers)
      {
        var driver = (IMessageBrokerDriver)Activator.CreateInstance(broker.DriverType, this, broker.DriverConfiguration) ??
                     throw new PoezdOperationException(
                       $"Can not create a message broker driver {broker.DriverType.FullName} during starting of the message router.");

        _brokers.Add(new MessageBroker(driver, broker));
      }
    }

    private MessageHandlingPipeline BuildPipeline(
      MessageBrokerConfiguration brokerConfiguration,
      PublicApiConfiguration publicApiConfiguration)
    {
      var pipeline = new MessageHandlingPipeline();
      var brokerPipelineConfigurator = GetBrokerPipelineConfigurator(brokerConfiguration);
      brokerPipelineConfigurator.ConfigurePipeline(pipeline);

      var publicApiPipelineConfigurator = GetPublicApiPipelineConfigurator(publicApiConfiguration);
      publicApiPipelineConfigurator.ConfigurePipeline(pipeline);
      return pipeline;
    }

    private PublicApiConfiguration GetPublicApiConfiguration(MessageBrokerConfiguration brokerConfiguration, string queueName)
    {
      var queueNameMatcher = (IQueueNameMatcher)_serviceProvider.GetService(brokerConfiguration.QueueNameMatcherType);
      var configuration = brokerConfiguration.PublicApis.FirstOrDefault(
        api => api.QueueNamePatterns.Any(queueNamePattern => queueNameMatcher.DoesMatch(queueName, queueNamePattern)));
      if (configuration != null)
      {
        return configuration;
      }

      _logger.LogDebug(
        $"The queue '{queueName}' for '{brokerConfiguration.Id}' broker isn't configured " +
        "but a message received from this queue.");
      return null;
    }

    private IPipelineConfigurator GetBrokerPipelineConfigurator(MessageBrokerConfiguration brokerConfiguration)
    {
      if (brokerConfiguration.PipelineConfiguratorType == null)
      {
        throw new PoezdConfigurationException(
          $"Broker with ID '{brokerConfiguration.Id}' has no configured pipeline configurator. You should use " +
          $"{nameof(MessageBrokerConfigurator)}.{nameof(MessageBrokerConfigurator.WithPipelineConfigurator)} " +
          "method to set pipeline configurator CLR-type.");
      }

      return (IPipelineConfigurator)_serviceProvider.GetService(
        brokerConfiguration.PipelineConfiguratorType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the message broker pipeline configurator of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private IPipelineConfigurator GetPublicApiPipelineConfigurator(PublicApiConfiguration publicApiConfiguration)
    {
      if (publicApiConfiguration.PipelineConfiguratorType == null)
      {
        throw new PoezdConfigurationException(
          $"Public API with ID '{publicApiConfiguration.Id}' has no configured pipeline configurator. You should use " +
          $"{nameof(PublicApiConfigurator)}.{nameof(PublicApiConfigurator.WithPipelineConfigurator)} " +
          "method to set pipeline configurator CLR-type.");
      }

      return (IPipelineConfigurator)_serviceProvider.GetService(
        publicApiConfiguration.PipelineConfiguratorType,
        type => new PoezdConfigurationException(
          $"Can not get instance of the public API pipeline configurator of type '{type.FullName}'. " +
          "You should register this type in DI-container."));
    }

    private bool _isStarted;
    private readonly List<MessageBroker> _brokers = new List<MessageBroker>();
    private readonly ILogger<MessageRouter> _logger;
    private readonly MessageRouterConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
