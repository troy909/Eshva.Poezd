#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public sealed partial class MessageRouter
  {
    private class NotStartedState : IMessageRouter
    {
      public NotStartedState(MessageRouter router)
      {
        _router = router;
      }

      public void Dispose() => _router.SetCurrentState(new DisposedState());

      public async Task Start(CancellationToken cancellationToken = default)
      {
        EnsureConfigurationValid();

        try
        {
          InitializeMessageBrokers();

          var starters = _router._brokers
            .Where(broker => !broker.Configuration.HasNoIngress)
            .Select(
              broker => broker.StartConsumeMessages(
                broker.Ingress.Apis.SelectMany(api => api.GetQueueNamePatterns()),
                cancellationToken));
          // TODO: Are exceptions here handled correctly?
          await Task.WhenAll(starters);
          _router.SetCurrentState(new StartedState(_router));
        }
        catch (Exception exception)
        {
          throw new PoezdOperationException(
            "Unable to start message router due an error. Inspect the inner exception for detailed information.",
            exception);
        }
      }

      public Task RouteIngressMessage(
        string brokerId,
        string queueName,
        DateTimeOffset receivedOnUtc,
        object key,
        object payload,
        IReadOnlyDictionary<string, string> metadata) =>
        throw new PoezdOperationException("Message router isn't started yet and can not route messages.");

      public Task RouteEgressMessage<TMessage>(
        TMessage message,
        string correlationId = default,
        string causationId = default,
        string messageId = default,
        DateTimeOffset timestamp = default) where TMessage : class =>
        throw new PoezdOperationException("Message router isn't started yet and can not route messages.");

      private void InitializeMessageBrokers()
      {
        _router._brokers.AddRange(
          _router._configuration.Brokers.Select(
            configuration =>
            {
              var broker = new MessageBroker(
                _router,
                configuration,
                _router._diContainerAdapter);
              broker.Initialize();
              return broker;
            }));
      }

      private void EnsureConfigurationValid()
      {
        var configurationErrors = _router._configuration.Validate().ToList();
        if (!configurationErrors.Any()) return;

        var message = new StringBuilder("Unable to start the message router due configuration errors:");
        configurationErrors.ForEach(error => message.AppendLine($"\t* {error}"));

        throw new PoezdConfigurationException(message.ToString());
      }

      private readonly MessageRouter _router;
    }
  }
}
