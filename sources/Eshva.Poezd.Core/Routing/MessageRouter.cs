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
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

      using (_diContainerAdapter.BeginScope())
      {
        var messageBroker = _brokers.Single(broker => broker.Id.Equals(brokerId, StringComparison.InvariantCultureIgnoreCase));
        var publicApi = messageBroker.GetApiByQueueName(queueName);

        var messageHandlingContext = BuildIngressMessageHandlingContext(
          messageBroker,
          publicApi,
          payload,
          metadata,
          queueName,
          receivedOnUtc);
        var pipeline = BuildIngressPipeline(messageBroker, publicApi);

        try
        {
          // TODO: Add timeout as a cancellation token and configuration its using router configuration fluent interface.
          return pipeline.Execute(messageHandlingContext);
        }
        catch (Exception exception)
        {
          throw new PoezdOperationException(
            "An error occurred during incoming message handling. Inspect the inner exceptions for more details.",
            exception);
        }
      }
    }

    /// <inheritdoc />
    public async Task RouteOutgoingMessage<TMessage>(
      TMessage message,
      string correlationId = default,
      string causationId = default,
      string messageId = default)
      where TMessage : class
    {
      var publicApi = _brokers.SelectMany(broker => broker.PublicApis).Single(api => api.MessageTypesRegistry.DoesOwn<TMessage>());
      var messageBroker = _brokers.Single(broker => broker.PublicApis.Contains(publicApi));

      using (_diContainerAdapter.BeginScope())
      {
        var context = BuildEgressMessageHandlingContext(
          message,
          messageBroker,
          publicApi,
          correlationId,
          causationId,
          messageId);

        var pipeline = BuildEgressPipeline(messageBroker, publicApi);

        try
        {
          // TODO: Add timeout as a cancellation token and configuration its using router configuration fluent interface.
          await pipeline.Execute(context);
          await PublishMessageWithDriver(messageBroker, context);
        }
        catch (Exception exception)
        {
          throw new PoezdOperationException(
            "An error occurred during outgoing message handling. Inspect the inner exceptions for more details.",
            exception);
        }
      }
    }

    private Task PublishMessageWithDriver(MessageBroker messageBroker, MessagePublishingContext context)
    {
      return messageBroker.Driver.Publish(context.Key, context.Payload, context.Metadata, context.QueueNames);
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

    private static ConcurrentPocket BuildIngressMessageHandlingContext(
      MessageBroker messageBroker,
      IPublicApi publicApi,
      byte[] payload,
      IReadOnlyDictionary<string, string> metadata,
      string queueName,
      DateTimeOffset receivedOnUtc)
    {
      try
      {
        // TODO: Replace with a IncomingMessageHandlingContext similar to MessagePublishingContext.
        var context = new ConcurrentPocket();
        context
          .Put(ContextKeys.Broker.MessageMetadata, metadata)
          .Put(ContextKeys.Broker.MessagePayload, payload)
          .Put(ContextKeys.Broker.QueueName, queueName)
          .Put(ContextKeys.Broker.ReceivedOnUtc, receivedOnUtc)
          .Put(ContextKeys.Broker.Itself, messageBroker)
          .Put(ContextKeys.PublicApi.Itself, publicApi);
        return context;
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException(
          "An error occurred during building an ingress message handling context. Inspect the inner exceptions for more details.",
          exception);
      }
    }

    private static MessageHandlingPipeline BuildIngressPipeline(MessageBroker messageBroker, IPublicApi publicApi)
    {
      try
      {
        var pipeline = new MessageHandlingPipeline();
        messageBroker.IngressEnterPipeFitter.AppendStepsInto(pipeline);
        publicApi.IngressPipeFitter.AppendStepsInto(pipeline);
        messageBroker.IngressExitPipeFitter.AppendStepsInto(pipeline);
        return pipeline;
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException(
          "An error occurred during building an ingress pipeline. Inspect the inner exceptions for more details.",
          exception);
      }
    }

    private static MessagePublishingContext BuildEgressMessageHandlingContext<TMessage>(
      TMessage message,
      MessageBroker messageBroker,
      IPublicApi publicApi,
      string correlationId,
      string causationId,
      string messageId)
      where TMessage : class
    {
      try
      {
        var context = new MessagePublishingContext
        {
          Message = message,
          Broker = messageBroker,
          PublicApi = publicApi,
          CorrelationId = correlationId,
          CausationId = causationId,
          MessageId = messageId
        };

        return context;
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException(
          "An error occurred during building an egress message handling context. Inspect the inner exceptions for more details.",
          exception);
      }
    }

    private static MessageHandlingPipeline BuildEgressPipeline(
      MessageBroker messageBroker,
      IPublicApi publicApi)
    {
      try
      {
        var pipeline = new MessageHandlingPipeline();
        messageBroker.EgressEnterPipeFitter.AppendStepsInto(pipeline);
        publicApi.EgressPipeFitter.AppendStepsInto(pipeline);
        messageBroker.EgressExitPipeFitter.AppendStepsInto(pipeline);

        return pipeline;
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException(
          "An error occurred during building an egress pipeline. Inspect the inner exceptions for more details.",
          exception);
      }
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

    private readonly List<MessageBroker> _brokers = new();
    private readonly MessageRouterConfiguration _configuration;
    private readonly IDiContainerAdapter _diContainerAdapter;
    private bool _isStarted;
  }
}
