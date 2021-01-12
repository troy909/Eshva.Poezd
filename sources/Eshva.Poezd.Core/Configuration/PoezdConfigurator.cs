#region Usings

using System;
using System.Threading;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.DependencyInjection;
using Eshva.Poezd.Core.Pipeline;
using Eshva.Poezd.Core.Routing;
using Eshva.Poezd.Core.Transport;
using JetBrains.Annotations;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public sealed class PoezdConfigurator
  {
    internal PoezdConfigurator([NotNull] IMessageHandlersFactory messageHandlersFactory)
    {
      if (messageHandlersFactory == null)
      {
        throw new ArgumentNullException(nameof(messageHandlersFactory));
      }

      _injectionist.Register(context => messageHandlersFactory);
    }

    /*
    public PoezdConfigurator Transport([NotNull] Action<StandardConfigurator<ITransport>> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new StandardConfigurator<ITransport>(_injectionist, _options));
      return this;
    }
    */

    /*
    public IMessageRouter Start()
    {
      VerifyRequirements();
      _injectionist.Register(_ => _options);
      _injectionist.Register(_ => new CancellationTokenSource());
      _injectionist.Register(context => context.Get<CancellationTokenSource>().Token);

      PossiblyRegisterDefault(
        context =>
        {
          var serializer = context.Get<ISerializer>();
          var loggerFactory = context.Get<ILoggerFactory>();
          var metadataHandler = context.Get<IMessegeMetadataHandler>();

          return new MessageProcessingPipeline()
                 .Append(new GetMessageMetadataStep(metadataHandler))
                 .Append(new DeserializeIncomingMessageStep(serializer))
                 .Append(new ActivateHandlersStep(context.Get<IMessageHandlersFactory>()))
                 .Append(new DispatchIncomingMessageStep(loggerFactory));
        });
    }
    */

    public PoezdConfigurator AddBus([NotNull] Action<BusConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new BusConfigurator());
      return this;
    }

    public PoezdConfigurator WithLogging(Action<LoggingConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new LoggingConfigurator());
      return this;
    }

    public PoezdConfigurator WithProcessManaging(Action<ProcessManagingConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new ProcessManagingConfigurator());
      return this;
    }

    /*
    public PoezdConfigurator WithMessageHandling(Action<MessageHandlingConfigurator> configurator)
    {
      if (configurator == null)
      {
        throw new ArgumentNullException(nameof(configurator));
      }

      configurator(new MessageHandlingConfigurator());
      return this;
    }
    */

    private void VerifyRequirements()
    {
      if (_hasBeenStarted)
      {
        throw new InvalidOperationException(
          "This configurator has already had .Start() called on it - this is not allowed, because it cannot be guaranteed " +
          "that configuration extensions make their registrations in a way that allows for being called more than once. " +
          "If you need to create multiple bus instances, please wrap the configuration from Configure.With(...) and on in " +
          "a function that you can call multiple times.");
      }

      if (!_injectionist.Has<ITransport>())
      {
        throw new PoezdConfigurationException(
          "No transport has been configured! You need to call .Transport(t => t.Use***) in order" +
          " to select which kind of queueing system you want to use to transport messages. If" +
          " you want something lightweight (possibly for testing?) you can use .Transport(t => t.UseInMemoryTransport(...))");
      }
    }

    private void PossiblyRegisterDefault<TService>(Func<IResolutionContext, TService> factoryMethod)
    {
      if (_injectionist.Has<TService>())
      {
        return;
      }

      _injectionist.Register(factoryMethod);
    }

    private void RegisterDecorator<TService>(Func<IResolutionContext, TService> factoryMethod) => _injectionist.Decorate(factoryMethod);

    private bool _hasBeenStarted;
    private readonly DependencyInjector _injectionist = new DependencyInjector(); // TODO: Rename to _dependencyInjector.
    private readonly Options _options = new Options();
  }
}
