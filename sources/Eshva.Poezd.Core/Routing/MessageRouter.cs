#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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
    public IReadOnlyCollection<IMessageBroker> Brokers => _brokers.AsReadOnly();

    /// <inheritdoc />
    public async Task Start(CancellationToken cancellationToken = default)
    {
      if (_isDisposed) throw new PoezdOperationException("It's not possible to start a disposed message router");
      if (_isStarted) throw new PoezdOperationException("The router is started already.");

      EnsureConfigurationValid();

      try
      {
        InitializeMessageBrokers();

        var starters = Brokers.Select(
          broker => broker.StartConsumeMessages(
            broker.Ingress.Apis.SelectMany(api => api.GetQueueNamePatterns()),
            cancellationToken));
        // TODO: Are exceptions here handled correctly?
        await Task.WhenAll(starters);
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
    public async Task RouteIngressMessage(
      string brokerId,
      string queueName,
      DateTimeOffset receivedOnUtc,
      object key,
      object payload,
      IReadOnlyDictionary<string, string> metadata)
    {
      // TODO: Message handling shouldn't stop but decision what to do with erroneous message should be carried to some API-related strategy.
      if (_isStopped)
        throw new PoezdOperationException("Further message handling is stopped due an error during handling another message.");
      if (payload == null) throw new ArgumentNullException(nameof(payload));
      if (metadata == null) throw new ArgumentNullException(nameof(metadata));
      if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));
      if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

      using (_diContainerAdapter.BeginScope())
      {
        // TODO: Move getting broker and API into a pipeline step.
        var messageBroker = _brokers.Single(broker => broker.Id.Equals(brokerId, StringComparison.InvariantCultureIgnoreCase));
        var api = messageBroker.Ingress.GetApiByQueueName(queueName);

        var messageHandlingContext = new MessageHandlingContext
        {
          Key = key,
          Payload = payload,
          Metadata = metadata,
          QueueName = queueName,
          ReceivedOnUtc = receivedOnUtc,
          Broker = messageBroker,
          Api = api
        };

        var pipeline = BuildIngressPipeline(messageBroker, api);

        try
        {
          // TODO: Add timeout as a cancellation token and configuration its using router configuration fluent interface.
          await pipeline.Execute(messageHandlingContext);
        }
        catch (Exception exception)
        {
          _isStopped = true;
          throw new PoezdOperationException(
            "An error occurred during ingress message handling. Inspect the inner exceptions for more details.",
            exception);
        }
      }
    }

    /// <inheritdoc />
    public async Task RouteEgressMessage<TMessage>(
      TMessage message,
      string correlationId = default,
      string causationId = default,
      string messageId = default)
      where TMessage : class
    {
      var egressApi = _brokers.SelectMany(broker => broker.Egress.Apis).Single(api => api.MessageTypesRegistry.DoesOwn<TMessage>());
      var messageBroker = _brokers.Single(broker => broker.Egress.Apis.Contains(egressApi));

      using (_diContainerAdapter.BeginScope())
      {
        var context = new MessagePublishingContext
        {
          Message = message,
          Broker = messageBroker,
          Api = egressApi,
          CorrelationId = correlationId,
          CausationId = causationId,
          MessageId = messageId
        };

        var pipeline = BuildEgressPipeline(messageBroker, egressApi);

        try
        {
          await pipeline.Execute(context);
          // TODO: Add timeout configuration using router configuration fluent interface.
          var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
          await PublishMessageWithDriver(context, timeout);
        }
        catch (Exception exception)
        {
          throw new PoezdOperationException(
            "An error occurred during message publishing. Inspect the inner exceptions for more details.",
            exception);
        }
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

    /// <inheritdoc />
    public void Dispose()
    {
      _brokers.ForEach(broker => broker.Dispose());
      _isDisposed = true;
    }

    private void InitializeMessageBrokers()
    {
      _brokers.AddRange(
        _configuration.Brokers.Select(
          configuration =>
          {
            var broker = new MessageBroker(
              this,
              configuration,
              _diContainerAdapter,
              _diContainerAdapter.GetService<IClock>());
            broker.Initialize();
            return broker;
          }));
    }

    private static Task PublishMessageWithDriver(
      MessagePublishingContext context,
      CancellationToken cancellationToken) =>
      context.Broker.Publish(context, cancellationToken);

    private static Pipeline<MessageHandlingContext> BuildIngressPipeline(IMessageBroker messageBroker, IIngressApi api)
    {
      try
      {
        var pipeline = new Pipeline<MessageHandlingContext>();
        messageBroker.Ingress.EnterPipeFitter.AppendStepsInto(pipeline);
        api.PipeFitter.AppendStepsInto(pipeline);
        messageBroker.Ingress.ExitPipeFitter.AppendStepsInto(pipeline);
        return pipeline;
      }
      catch (Exception exception)
      {
        throw new PoezdOperationException(
          "An error occurred during building an ingress pipeline. Inspect the inner exceptions for more details.",
          exception);
      }
    }

    private static Pipeline<MessagePublishingContext> BuildEgressPipeline(IMessageBroker messageBroker, IEgressApi api)
    {
      try
      {
        var pipeline = new Pipeline<MessagePublishingContext>();
        messageBroker.Egress.EnterPipeFitter.AppendStepsInto(pipeline);
        api.PipeFitter.AppendStepsInto(pipeline);
        messageBroker.Egress.ExitPipeFitter.AppendStepsInto(pipeline);

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

    private readonly List<IMessageBroker> _brokers = new();
    private readonly MessageRouterConfiguration _configuration;
    private readonly IDiContainerAdapter _diContainerAdapter;

    // TODO: Use the State pattern.
    private bool _isDisposed;
    private bool _isStarted;
    private bool _isStopped;
  }
}
