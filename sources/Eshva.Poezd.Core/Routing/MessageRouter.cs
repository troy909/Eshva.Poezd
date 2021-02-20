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
  public sealed class MessageRouter : IMessageRouter
  {
    internal MessageRouter(
      [NotNull] MessageRouterConfiguration configuration,
      [NotNull] IDiContainerAdapter diContainerAdapter)
    {
      _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
      _diContainerAdapter = diContainerAdapter ?? throw new ArgumentNullException(nameof(diContainerAdapter));
      _logger = (ILogger<MessageRouter>) _diContainerAdapter.GetService(typeof(ILogger<MessageRouter>)) ??
                throw new ArgumentException($"Can not get {nameof(ILogger<MessageRouter>)} implementation from the service provider.");
    }

    public IReadOnlyCollection<MessageBroker> Brokers => _brokers.AsReadOnly();

    public async Task Start(CancellationToken cancellationToken = default)
    {
      if (_isStarted) throw new PoezdOperationException("The router is started already.");

      ValidateConfiguration();

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

      using (_diContainerAdapter.BeginScope())
      {
        MessageHandlingPipeline pipeline;
        var messageHandlingContext = new ConcurrentPocket();
        try
        {
          var messageBroker = _brokers.Single(broker => broker.Id.Equals(brokerId, StringComparison.InvariantCultureIgnoreCase));
          var publicApi = messageBroker.GetApiByQueueName(queueName);

          pipeline = BuildIngressPipeline(messageBroker, publicApi);

          messageHandlingContext
            .Put(ContextKeys.Broker.Id, brokerId)
            .Put(ContextKeys.Broker.MessageMetadata, brokerMetadata)
            .Put(ContextKeys.Broker.MessagePayload, brokerPayload)
            .Put(ContextKeys.Broker.QueueName, queueName)
            .Put(ContextKeys.Broker.ReceivedOnUtc, receivedOnUtc)
            .Put(ContextKeys.Broker.Configuration, messageBroker.Configuration)
            .Put(ContextKeys.PublicApi.Id, publicApi.Id)
            .Put(ContextKeys.PublicApi.MessageTypesRegistry, publicApi.MessageTypesRegistry)
            .Put(ContextKeys.PublicApi.Configuration, publicApi.Configuration);
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
          // TODO: Add timeout as a cancellation token and configuration of it using router configuration fluent interface.
          return pipeline.Execute(messageHandlingContext);
        }
        catch (Exception exception)
        {
          _logger.LogError(
            exception,
            "An error occurred during message handling. See inner exception to find details about the error.");
          return Task.CompletedTask;
        }
      }
    }

    public static MessageRouterConfiguration Configure([NotNull] Action<MessageRouterConfigurator> configurator)
    {
      if (configurator == null) throw new ArgumentNullException(nameof(configurator));

      var poezdConfigurator = new MessageRouterConfigurator();
      configurator(poezdConfigurator);
      return poezdConfigurator.Configuration;
    }

    private void ValidateConfiguration()
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
      messageBroker.IngressEnterPipeFitter.Setup(pipeline);
      publicApi.IngressPipeFitter.Setup(pipeline);
      messageBroker.IngressExitPipeFitter.Setup(pipeline);
      return pipeline;
    }

    private readonly List<MessageBroker> _brokers = new List<MessageBroker>();
    private readonly MessageRouterConfiguration _configuration;
    private readonly IDiContainerAdapter _diContainerAdapter;
    private readonly ILogger<MessageRouter> _logger;

    private bool _isStarted;
    private const string NotWhitespace = "Value cannot be null or whitespace.";
  }
}
