#region Usings

using System;
using System.Collections;
using System.Collections.Generic;
using Eshva.Common;
using Eshva.Common.Collections;
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
      _messageHandlerType = messageHandlerType ?? throw new ArgumentNullException(nameof(messageHandlerType));

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

    public IEnumerable<IHandleMessage> GetHandlersOfMessage(Type messageType, IMessageHandlingContext messageHandlingContext) =>
      GetInstances(CreateContainerScope(messageHandlingContext), messageType);

    protected abstract IHandleMessage CreatePoezdMessageHandler(object applicationHandler);

    private Scope CreateContainerScope(IMessageHandlingContext messageHandlingContext)
    {
      const string CurrentSimpleInjectorScope = "current-simpleinjector-scope";
      var scope = messageHandlingContext.TakeOrPut(
        CurrentSimpleInjectorScope,
        () =>
        {
          var newScope = AsyncScopedLifestyle.BeginScope(_container);
          messageHandlingContext.SubscribeOn.Disposed(context => newScope.Dispose());
          return newScope;
        });
      return scope;
    }

    private IEnumerable<IHandleMessage> GetInstances(IServiceProvider provider, Type messageType)
    {
      var handlerType = _messageHandlerType.MakeGenericType(messageType);
      var handlerCollectionType = typeof(IEnumerable<>).MakeGenericType(handlerType);
      foreach (var handler in (IEnumerable) provider.GetService(handlerCollectionType))
      {
        yield return CreatePoezdMessageHandler(handler);
      }
    }

    private readonly Container _container;

    private readonly Type _messageHandlerType;
  }
}
