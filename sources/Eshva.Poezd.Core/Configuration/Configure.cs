#region Usings

using System;
using Eshva.Poezd.Core.Activation;

#endregion


namespace Eshva.Poezd.Core.Configuration
{
  public static class Configure
  {
    /// <summary>
    /// Call this method with the chosen implementation of <see cref="IMessageHandlersFactory"/>
    /// in order to start configuring a Poezd instance.
    /// </summary>
    public static PoezdConfigurator With(IMessageHandlersFactory handlersFactory)
    {
      if (handlersFactory == null) throw new ArgumentNullException(nameof(handlersFactory), HandlerActivatorNotSpecifiedErrorMessage);

      return new PoezdConfigurator(handlersFactory);
    }

    private static readonly string HandlerActivatorNotSpecifiedErrorMessage =
      "Please remember to pass a handlers factory to the .With(..) method." + Environment.NewLine +
      "The handlers factory is responsible for looking up handlers for incoming messages, which makes for " +
      "a pretty good place to use an adapter for an DI-container (because then your handlers can have " +
      "dependencies injected).";
  }
}
