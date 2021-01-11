#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Eshva.Common;
using Eshva.Poezd.Core.Activation;
using Eshva.Poezd.Core.MessageHandling;
using JetBrains.Annotations;
using SimpleInjector;
using SimpleInjector.Lifestyles;

#endregion


namespace Eshva.Poezd.SimpleInjectorCoupling
{
  public abstract class MessageHandlerAdapter : IMessageHandlersFactory
  {
    protected MessageHandlerAdapter([NotNull] Container container, [NotNull] Type messageHandlerType)
    {
      _container = container ?? throw new ArgumentNullException(nameof(container));
      MessageHandlerType = messageHandlerType ?? throw new ArgumentNullException(nameof(messageHandlerType));

      var doesLookLikeMessageHandler = messageHandlerType.IsGenericTypeDefinition &&
                                       messageHandlerType.GenericTypeArguments.Length == 0 &&
                                       messageHandlerType.GetGenericArguments().Length == 1;
      if (!doesLookLikeMessageHandler)
      {
        throw new ArgumentException(
          "The message handler type should be an open generic with 1 type parameter." +
          "For example: IHandler<> where IHandler have type definition definition: interface IHandler<T>.");
      }
    }

    public Type MessageHandlerType { get; }

    public Task<IEnumerable<IHandleMessage<TMessage>>> GetHandlersOfMessage<TMessage>(
      TMessage message,
      ITransactionContext transactionContext)
      where TMessage : class =>
      Task.FromResult(GetInstances<TMessage>(CreateContainerScope(transactionContext)));

    protected abstract IHandleMessage<TMessage> CreatePoezdMessageHandler<TMessage>(object handler);

    private Scope CreateContainerScope(ITransactionContext transactionContext)
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
      return scope;
    }

    private IEnumerable<IHandleMessage<TMessage>> GetInstances<TMessage>(IServiceProvider provider)
    {
      var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(MessageHandlerType);
      foreach (var handler in (IEnumerable)provider.GetService(handlerCollectionType))
      {
        yield return CreatePoezdMessageHandler<TMessage>(handler);
      }
    }

    private readonly Container _container;
  }
}
