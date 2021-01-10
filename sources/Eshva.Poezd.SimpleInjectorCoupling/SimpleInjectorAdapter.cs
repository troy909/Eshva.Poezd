#region Usings

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.MessageHandling;
using Eshva.Poezd.Core.Transport;
using JetBrains.Annotations;
using SimpleInjector;
using SimpleInjector.Lifestyles;

#endregion


namespace Eshva.Poezd.SimpleInjectorCoupling
{
  public sealed class SimpleInjectorAdapter : IMessageHandlersFactory
  {
    public SimpleInjectorAdapter([NotNull] Container container)
    {
      _container = container ?? throw new ArgumentNullException(nameof(container));
    }

    public Task<IReadOnlyCollection<IHandleMessage<TMessage>>> GetHandlersOfMessage<TMessage>(
      TMessage message,
      ITransactionContext transactionContext)
      where TMessage : class
    {
      const string CurrentSimpleInjectorScope = "current-simpleinjector-scope";

      var scope = transactionContext.GetOrAdd(
        CurrentSimpleInjectorScope,
        () =>
        {
          var newScope = AsyncScopedLifestyle.BeginScope(_container);
          transactionContext.SubscribeOn.Disposed(context => newScope.Dispose());
          return newScope;
        });

      return Task.FromResult(
        TryGetInstance<IReadOnlyCollection<IHandleMessage<TMessage>>>(scope, out var handlerInstances)
          ? AdaptHandlers(handlerInstances)
          : new IHandleMessage<TMessage>[0]);
    }

    public Task<IReadOnlyCollection<IHandleMessage<>>>

    private IReadOnlyCollection<IHandleMessage<TMessage>> AdaptHandlers<TMessage>(
      IReadOnlyCollection<IHandleMessage<TMessage>> handlerInstances)
      where TMessage : class
    {
      return null;
    }

    private static bool TryGetInstance<TService>(IServiceProvider provider, out TService instance)
      where TService : class
    {
      instance = (TService)provider.GetService(typeof(TService));
      return instance != null;
    }

    private readonly Container _container;
  }
}
