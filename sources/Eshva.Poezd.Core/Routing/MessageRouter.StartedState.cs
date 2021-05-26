#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Common;
using Eshva.Poezd.Core.Pipeline;

#endregion

namespace Eshva.Poezd.Core.Routing
{
  public sealed partial class MessageRouter
  {
    private class StartedState : IMessageRouter
    {
      public StartedState(MessageRouter router)
      {
        _router = router;
      }

      public void Dispose()
      {
        _router._brokers.ForEach(broker => broker.Dispose());
        _router.SetCurrentState(new DisposedState());
      }

      public Task Start(CancellationToken cancellationToken = default) =>
        throw new PoezdOperationException("The router is started already.");

      public async Task RouteIngressMessage(
        string brokerId,
        string queueName,
        DateTimeOffset receivedOnUtc,
        object key,
        object payload,
        IReadOnlyDictionary<string, string> metadata)
      {
        if (_isStopped)
          throw new PoezdOperationException("Further message handling is stopped due an error during handling another message.");
        if (payload == null) throw new ArgumentNullException(nameof(payload));
        if (metadata == null) throw new ArgumentNullException(nameof(metadata));
        if (string.IsNullOrWhiteSpace(brokerId)) throw new ArgumentNullException(nameof(brokerId));
        if (string.IsNullOrWhiteSpace(queueName)) throw new ArgumentNullException(nameof(queueName));

        using (_router._diContainerAdapter.BeginScope())
        {
          var messageBroker = _router._brokers.Single(broker => broker.Id.Equals(brokerId, StringComparison.InvariantCultureIgnoreCase));
          if (messageBroker.Configuration.HasNoIngress)
            throw new PoezdOperationException("Driver shouldn't route ingress messages if no ingress configured.");

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
            // TODO: Message handling shouldn't stop but decision what to do with erroneous message should be carried to
            // some API-related strategy.
            _isStopped = true;
            throw new PoezdOperationException(
              "An error occurred during ingress message handling. Inspect the inner exceptions for more details.",
              exception);
          }
        }
      }

      public Task RouteEgressMessage<TMessage>(
        TMessage message,
        string correlationId = default,
        string causationId = default,
        string messageId = default,
        DateTimeOffset timestamp = default) where TMessage : class
      {
        var apis = _router._brokers.SelectMany(broker => broker.Egress.Apis).Where(api => api.MessageTypesRegistry.DoesOwn<TMessage>())
          .ToArray();
        if (!apis.Any())
          throw new PoezdOperationException($"Unable to find destinations for message of type {message.GetType().FullName}.");

        var tasks = apis.Select(
          async api =>
          {
            var context = new MessagePublishingContext
            {
              Message = message,
              Broker = _router._brokers.Single(broker => broker.Egress.Apis.Contains(api)),
              Api = api,
              CorrelationId = correlationId,
              CausationId = causationId,
              MessageId = messageId,
              Timestamp = timestamp
            };

            var pipeline = BuildEgressPipeline(_router._brokers.Single(broker => broker.Egress.Apis.Contains(api)), api);

            try
            {
              await pipeline.Execute(context);
              // TODO: Add timeout configuration using router configuration fluent interface.
              var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(value: 5)).Token;
              await context.Broker.Publish(context, timeout);
            }
            catch (Exception exception)
            {
              throw new PoezdOperationException(
                "An error occurred during message publishing. Inspect the inner exceptions for more details.",
                exception);
            }
          });

        return Task.WhenAll(tasks);
      }

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

      private readonly MessageRouter _router;
      private bool _isStopped;
    }
  }
}
