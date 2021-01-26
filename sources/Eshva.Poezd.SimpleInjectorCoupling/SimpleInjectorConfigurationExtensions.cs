#region Usings

using System;
using System.Collections.Generic;
using Eshva.Poezd.Core.MessageHandling;
using SimpleInjector;

#endregion


namespace Eshva.Poezd.SimpleInjectorCoupling
{
  public static class SimpleInjectorConfigurationExtensions
  {
    /// <summary>
    /// Registers <typeparamref name="THandler" /> as a handler of messages of type <typeparamref name="TMessage" />
    /// </summary>
    public static void RegisterHandlers<TMessage, THandler>(this Container container)
      where THandler : IHandleMessage where TMessage : class
    {
      RegisterHandlers(
        container,
        typeof(IHandleMessage),
        new[] {typeof(THandler)});
    }

    /// <summary>
    /// Registers <typeparamref name="THandler1" /> and <typeparamref name="THandler2" /> as handlers of messages of type
    /// <typeparamref name="TMessage" />
    /// </summary>
    public static void RegisterHandlers<TMessage, THandler1, THandler2>(this Container container)
      where THandler1 : IHandleMessage
      where THandler2 : IHandleMessage
      where TMessage : class
    {
      RegisterHandlers(
        container,
        typeof(IHandleMessage),
        new[] {typeof(THandler1), typeof(THandler2)});
    }

    /// <summary>
    /// Registers <typeparamref name="THandler1" /> and <typeparamref name="THandler2" /> and <typeparamref name="THandler3" />
    /// as handlers of messages of type <typeparamref name="TMessage" />
    /// </summary>
    public static void RegisterHandlers<TMessage, THandler1, THandler2, THandler3>(this Container container)
      where THandler1 : IHandleMessage
      where THandler2 : IHandleMessage
      where THandler3 : IHandleMessage
      where TMessage : class
    {
      RegisterHandlers(
        container,
        typeof(IHandleMessage),
        new[] {typeof(THandler1), typeof(THandler2), typeof(THandler3)});
    }

    /*
    public static void ConfigurePoezdRouter(this Container container, Func<PoezdConfigurator, IMessageRouter> configurator)
    {
      var isMessageRouterRegistered = container.GetCurrentRegistrations()
                                               .Select(instanceProducer => instanceProducer.ServiceType)
                                               .Any(type => type == typeof(IMessageRouter));
      if (isMessageRouterRegistered)
      {
        throw new InvalidOperationException("Cannot register IMessageRouter in the container because it has already been registered.");
      }

      container.Register(() => configurator(Configure.With(new VentureMessageHandlerFactory(container))));
    }
    */

    private static void RegisterHandlers(
      Container container,
      Type messageHandlerType,
      IEnumerable<Type> concreteHandlerTypes) =>
      container.Collection.Register(
        messageHandlerType,
        concreteHandlerTypes,
        Lifestyle.Scoped);
  }
}
