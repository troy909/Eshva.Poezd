#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Common.Collections;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Configuration;
using Eshva.Poezd.Core.Pipeline;
using JetBrains.Annotations;
using Microsoft.Extensions.Logging;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  /// <summary>
  /// The Poezd message router. The core part of Poezd.
  /// </summary>
  public sealed class MessageRouter : IMessageRouter
  {
    /// <summary>
    /// Constructs a new instance of message router.
    /// </summary>
    /// <param name="configuration">
    /// The message router configuration.
    /// </param>
    /// <param name="diContainerAdapter">
    /// DI-container adapter.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// One of arguments is not specified.
    /// </exception>
    internal MessageRouter(
      [NotNull] MessageRouterConfiguration configuration,
      [NotNull] IDiContainerAdapter diContainerAdapter)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _diContainerAdapter = diContainerAdapter ?? throw new ArgumentNullException(nameof(diContainerAdapter));
      _logger = (ILogger<MessageRouter>) _diContainerAdapter.GetService(typeof(ILogger<MessageRouter>)) ??
                throw new ArgumentException($"Can not get {nameof(ILogger<MessageRouter>)} implementation from the service provider.");
    }

    /// <inheritdoc />
    public IReadOnlyCollection<MessageBroker> Brokers => _brokers.AsReadOnly();

    /// <inheritdoc />
    public async Task Start(CancellationToken cancellationToken = default)
    {
      if (_isStarted) throw new PoezdOperationException("The router is started already.");

      EnsureConfigurationValid();

      try
      {
        ConfigureMessageBrokerDrivers();
        foreach (var broker in _brokers)
        {
          broker.Driver.Initialize(
            this,
            broker.Id,
            broker.DriverConfiguration);
          var queueNamePatterns = broker.PublicApis.SelectMany(api => api.GetQueueNamePatterns());
          await broker.Driver.StartConsumeMessages(queueNamePatterns, cancellationToken);
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

    /// <inheritdoc />
    public Task RouteIncomingMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata)
    {
      if (payload == null) throw new ArgumentNullException(nameof(payload));
      if (metadata == null) throw new ArgumentNullException(nameof(metadata));
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentException(NotWhitespace, nameof(brokerId));
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentException(NotWhitespace, nameof(queueName));

      using (_diContainerAdapter.BeginScope())
      {
        MessageHandlingPipeline pipeline;
        var messageHandlingContext = new ConcurrentPocket();
        try
        {
          var messageBroker = _brokers.Single(broker => broker.Id.Equals(brokerId, StringComparison.InvariantCultureIgnoreCase));
          var publicApi = messageBroker.GetApiByQueueName(queueName);

          pipeline = BuildIngressPipeline(messageBroker, publicApi);

          // TODO: Replace all ContextKeys.Broker.* with ContextKeys.Broker.Itself.
          // TODO: Replace all ContextKeys.PublicApi.* with ContextKeys.PublicApi.Itself.
          messageHandlingContext
            .Put(ContextKeys.Broker.Id, brokerId)
            .Put(ContextKeys.Broker.MessageMetadata, metadata)
            .Put(ContextKeys.Broker.MessagePayload, payload)
            .Put(ContextKeys.Broker.QueueName, queueName)
            .Put(ContextKeys.Broker.ReceivedOnUtc, receivedOnUtc)
            .Put(ContextKeys.Broker.Configuration, messageBroker.Configuration)
            .Put(ContextKeys.PublicApi.Id, publicApi.Id)
            .Put(ContextKeys.PublicApi.MessageTypesRegistry, publicApi.MessageTypesRegistry)
            .Put(ContextKeys.PublicApi.HandlerRegistry, publicApi.HandlerRegistry)
            .Put(ContextKeys.PublicApi.Configuration, publicApi.Configuration);
        }
        catch (Exception exception)
        {
          _logger.LogError(exception, "An error occurred during preparing to handle an incoming message.");
          throw;
        }

        try
        {
          // TODO: Add timeout as a cancellation token and configuration of it using router configuration fluent interface.
          return pipeline.Execute(messageHandlingContext);
        }
        catch (Exception exception)
        {
          _logger.LogError(exception, "An error occurred during incoming message handling.");
          throw;
        }
      }
    }

    /// <inheritdoc />
    public Task RouteOutgoingMessage<TMessage>(
      TMessage message,
      string messageId = default,
      string correlationId = default,
      string causationId = default)
      where TMessage : class
    {
      var messageHandlingContext = new ConcurrentPocket();

      MessageHandlingPipeline pipeline;
      try
      {
        using (_diContainerAdapter.BeginScope())
        {
          var publicApi = _brokers.SelectMany(broker => broker.PublicApis).Single(api => api.MessageTypesRegistry.DoesOwn<TMessage>());
          var messageBroker = _brokers.Single(broker => broker.PublicApis.Contains(publicApi));
          var messageType = message.GetType();
          var messageTypeName = publicApi.MessageTypesRegistry.GetMessageTypeNameByItsMessageType<TMessage>();

          pipeline = BuildEgressPipeline(messageBroker, publicApi);
          messageHandlingContext
            .Put(ContextKeys.Broker.Itself, messageBroker)
            .Put(ContextKeys.PublicApi.Itself, messageBroker)
            .Put(ContextKeys.Application.MessagePayload, message)
            .Put(ContextKeys.Application.MessageType, messageType)
            .Put(ContextKeys.Application.MessageTypeName, messageTypeName);
          if (!string.IsNullOrWhiteSpace(messageId)) messageHandlingContext.Put(ContextKeys.Application.MessageId, messageId);
          if (!string.IsNullOrWhiteSpace(correlationId)) messageHandlingContext.Put(ContextKeys.Application.CorrelationId, correlationId);
          if (!string.IsNullOrWhiteSpace(causationId)) messageHandlingContext.Put(ContextKeys.Application.CausationId, causationId);
        }
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An error occurred during preparing to handle an outgoing message.");
        throw;
      }

      try
      {
        return pipeline.Execute(messageHandlingContext);
      }
      catch (Exception exception)
      {
        _logger.LogError(exception, "An error occurred during outgoing message handling.");
        throw;
      }
    }

    /// <summary>
    /// Provides the message router configuration.
    /// </summary>
    /// <param name="configurator">
    /// The message router configurator.
    /// </param>
    /// <returns>
    /// Message router configuration.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Configurator is not specified.
    /// </exception>
    public static MessageRouterConfiguration Configure([NotNull] Action<MessageRouterConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var poezdConfigurator = new MessageRouterConfigurator();
      configurator(poezdConfigurator);
      return poezdConfigurator.Configuration;
    }

    private void EnsureConfigurationValid()
    {
      var configurationErrors = _configuration.Validate().ToList();
      if (!configurationErrors.Any()) return;

      var message = new StringBuilder("Unable to start the message router due configuration errors:");
      configurationErrors.ForEach(error => message.AppendLine($"\t* {error}"));

      throw new PoezdConfigurationException(message.ToString());
    }

    private void ConfigureMessageBrokerDrivers()
    {
      foreach (var broker in _configuration.Brokers)
      {
        try
        {
          var driverFactory = (IMessageBrokerDriverFactory) _diContainerAdapter.GetService(broker.DriverFactoryType);
          _brokers.Add(
            new MessageBroker(
              driverFactory.Create(),
              broker,
              _diContainerAdapter));
        }
        catch (Exception exception)
        {
          throw new PoezdOperationException(
            $"Can not create a message broker driver using {broker.DriverFactoryType.FullName} during starting of the message router.",
            exception);
        }
      }
    }

    private static MessageHandlingPipeline BuildIngressPipeline(MessageBroker messageBroker, IPublicApi publicApi)
    {
      var pipeline = new MessageHandlingPipeline();
      messageBroker.IngressEnterPipeFitter.AppendStepsInto(pipeline);
      publicApi.IngressPipeFitter.AppendStepsInto(pipeline);
      messageBroker.IngressExitPipeFitter.AppendStepsInto(pipeline);
      return pipeline;
    }

    private static MessageHandlingPipeline BuildEgressPipeline(MessageBroker messageBroker, IPublicApi publicApi)
    {
      var pipeline = new MessageHandlingPipeline();
      messageBroker.EgressEnterPipeFitter.AppendStepsInto(pipeline);
      publicApi.EgressPipeFitter.AppendStepsInto(pipeline);
      messageBroker.EgressExitPipeFitter.AppendStepsInto(pipeline);
      return pipeline;
    }

    private readonly List<MessageBroker> _brokers = new();
    private readonly MessageRouterConfiguration _configuration;
    private readonly IDiContainerAdapter _diContainerAdapter;
    private readonly ILogger<MessageRouter> _logger;

    private bool _isStarted;
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
